
import sys
import os
import re

import argparse

from mtm.log.LogStreamFile import LogStreamFile
from mtm.log.LogStreamConsole import LogStreamConsole

from mtm.util.ZipHelper import ZipHelper

from mtm.util.ScriptRunner import ScriptRunner

from mtm.util.ProcessRunner import ProcessRunner
from mtm.util.SystemHelper import SystemHelper
from mtm.util.VarManager import VarManager
from mtm.config.Config import Config
from mtm.log.Logger import Logger
from mtm.util.VisualStudioHelper import VisualStudioHelper
from mtm.util.UnityHelper import UnityHelper, Platforms

from mtm.util.Assert import *

import mtm.ioc.Container as Container
from mtm.ioc.Inject import Inject

ScriptDir = os.path.dirname(os.path.realpath(__file__))
RootDir = os.path.realpath(os.path.join(ScriptDir, '../../../..'))

class Runner:
    _scriptRunner = Inject('ScriptRunner')
    _unityHelper = Inject('UnityHelper')
    _log = Inject('Logger')
    _sys = Inject('SystemHelper')
    _varManager = Inject('VarManager')

    def __init__(self):
        self._platform = Platforms.Windows

    def run(self, args):
        self._args = args
        success = self._scriptRunner.runWrapper(self._runInternal)

        if not success:
            sys.exit(1)

    def _runBuilds(self):

        if self._args.clearOutput:
            self._log.heading("Clearing output directory")
            self._sys.clearDirectoryContents('[OutputRootDir]')

        if self._args.buildType == 'all' or self._args.buildType == 'win35':
            self._log.heading("Building windows 3.5")
            self._platform = Platforms.Windows
            self._enableNet35()
            self._createBuild()

        if self._args.buildType == 'all' or self._args.buildType == 'win46':
            self._log.heading("Building windows 4.6")
            self._platform = Platforms.Windows
            self._enableNet46()
            self._createBuild()

        if self._args.buildType == 'all' or self._args.buildType == 'wsa35':
            self._log.heading("Building WindowsStoreApp 3.5 .net")
            self._platform = Platforms.WindowsStoreApp
            self._enableNet35()
            self._enableNetBackend()
            self._createBuild()

        if self._args.buildType == 'all' or self._args.buildType == 'wsa46':
            self._log.heading("Building WindowsStoreApp 4.6 .net")
            self._platform = Platforms.WindowsStoreApp
            self._enableNet46()
            self._enableNetBackend()
            self._createBuild()

        if self._args.buildType == 'all' or self._args.buildType == 'wsa46il2cpp':
            self._log.heading("Building WindowsStoreApp 4.6 il2cpp")
            self._platform = Platforms.WindowsStoreApp
            self._enableNet46()
            self._enableIl2cpp()
            self._createBuild()

        if self._args.buildType == 'all' or self._args.buildType == 'wsa35il2cpp':
            self._log.heading("Building WindowsStoreApp 3.5 il2cpp")
            self._platform = Platforms.WindowsStoreApp
            self._enableNet35()
            self._enableIl2cpp()
            self._createBuild()

        if self._args.buildType == 'all' or self._args.buildType == 'webgl35':
            self._log.heading("Building WebGl 3.5")
            self._platform = Platforms.WebGl
            self._enableNet35()
            self._createBuild()
            self._sys.copyFile('[WebGlTemplate]', '[OutputRootDir]/WebGl/Net35/Web.config')

        if self._args.buildType == 'all' or self._args.buildType == 'webgl46':
            self._log.heading("Building WebGl 4.6")
            self._platform = Platforms.WebGl
            self._enableNet46()
            self._createBuild()
            self._sys.copyFile('[WebGlTemplate]', '[OutputRootDir]/WebGl/Net46/Web.config')

        # TODO
        #self._log.heading("Building Ios")
        #self._platform = Platforms.Ios
        #self._createBuild()

        #self._log.heading("Building Android")
        #self._platform = Platforms.Android
        #self._createBuild()

    def _runTests(self):
        self._runUnityTests('editmode')
        self._runUnityTests('playmode')

    def _runUnityTests(self, testPlatform):

        self._log.heading('Running unity {0} unit tests'.format(testPlatform))

        resultPath = self._varManager.expandPath('[TempDir]/UnityUnitTestsResults.xml').replace('\\', '/')
        self._sys.removeFileIfExists(resultPath)

        try:
            self._unityHelper.runEditorFunctionRaw('[UnityProjectPath]', None, self._platform, '-runTests -batchmode -nographics -testResults "{0}" -testPlatform {1}'.format(resultPath, testPlatform))

        except UnityReturnedErrorCodeException as e:

            if self._sys.fileExists(resultPath):
                # Print out the test error info
                outRoot = ET.parse(resultPath)
                for item in outRoot.findall('.//failure/..'):
                    name = item.get('name')
                    self._log.error("Unit test failed for '{0}'.", name)

                    failure = item.find('./failure')
                    self._log.error("Message: {0}", failure.find('./message').text.strip())

                    stackTrace = failure.find('./stack-trace')

                    if stackTrace is not None:
                        self._log.error("Stack Trace: {0}", stackTrace.text.strip())
            raise

        outRoot = ET.parse(resultPath)
        total = outRoot.getroot().get('total')
        self._log.info("Processed {0} {1} tests without errors", total, testPlatform)

    def _runInternal(self):

        if self._args.runTests:
            self._runTests()

        if self._args.runBuilds:
            self._runBuilds()

        if self._args.openUnity:
            self._openUnity()

    def _createBuild(self):
        self._log.info("Creating build")
        self._runEditorFunction('BuildRelease')
        #self._runEditorFunction('BuildDebug')

    def _enableNet46(self):
        self._log.info("Changing runtime to .net 4.6")
        self._runEditorFunction('EnableNet46')

    def _enableNet35(self):
        self._log.info("Changing runtime to .net 3.5")
        self._runEditorFunction('EnableNet35')

    def _enableNetBackend(self):
        self._log.info("Changing backend to .net")
        self._runEditorFunction('EnableBackendNet')

    def _enableIl2cpp(self):
        self._log.info("Enabling il2cpp")
        self._runEditorFunction('EnableBackendIl2cpp')

    def _openUnity(self):
        self._unityHelper.openUnity('[UnityProjectPath]', self._platform)

    def _runEditorFunction(self, functionName):
        self._log.info("Calling SampleBuilder." + functionName)
        self._unityHelper.runEditorFunction('[UnityProjectPath]', 'Zenject.Internal.SampleBuilder.' + functionName, self._platform)

def installBindings():

    config = {
        'PathVars': {
            'ScriptDir': ScriptDir,
            'RootDir': RootDir,
            'BuildDir': '[RootDir]/Build',
            'TempDir': '[RootDir]/Temp',
            'WebGlTemplate': '[ScriptDir]/web_config_template.xml',
            'OutputRootDir': '[RootDir]/SampleBuilds',
            'UnityExePath': 'D:/Utils/Unity/Installs/2018.1.0f2/Editor/Unity.exe',
            'LogPath': '[BuildDir]/Log.txt',
            'UnityProjectPath': '[RootDir]/UnityProject',
            'MsBuildExePath': 'C:/Windows/Microsoft.NET/Framework/v4.0.30319/msbuild.exe'
        },
        'Compilation': {
            'UseDevenv': False
        },
    }
    Container.bind('Config').toSingle(Config, [config])

    Container.bind('LogStream').toSingle(LogStreamFile)
    Container.bind('LogStream').toSingle(LogStreamConsole, True, False)

    Container.bind('ScriptRunner').toSingle(ScriptRunner)
    Container.bind('VarManager').toSingle(VarManager)
    Container.bind('SystemHelper').toSingle(SystemHelper)
    Container.bind('Logger').toSingle(Logger)
    Container.bind('ProcessRunner').toSingle(ProcessRunner)
    Container.bind('ZipHelper').toSingle(ZipHelper)
    Container.bind('VisualStudioHelper').toSingle(VisualStudioHelper)
    Container.bind('UnityHelper').toSingle(UnityHelper)

if __name__ == '__main__':

    if (sys.version_info < (3, 0)):
        print('Wrong version of python!  Install python 3 and try again')
        sys.exit(2)

    parser = argparse.ArgumentParser(description='Create Sample')
    parser.add_argument('-ou', '--openUnity', action='store_true', help='')
    parser.add_argument('-c', '--clearOutput', action='store_true', help='')
    parser.add_argument('-rt', '--runTests', action='store_true', help='')
    parser.add_argument('-rb', '--runBuilds', action='store_true', help='')
    parser.add_argument('-t', '--buildType', type=str, default='win35', choices=['win35', 'win46', 'wsa35', 'wsa46', 'wsa46il2cpp', 'wsa35il2cpp', 'webgl35', 'webgl46', 'all'], help='')

    args = parser.parse_args(sys.argv[1:])

    installBindings()

    Runner().run(args)


