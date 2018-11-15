import os
import time

from mtm.log.LogWatcher import LogWatcher

import mtm.ioc.Container as Container
from mtm.ioc.Inject import Inject
import mtm.ioc.IocAssertions as Assertions

from mtm.util.Assert import *
import mtm.util.MiscUtil as MiscUtil

from mtm.util.SystemHelper import ProcessErrorCodeException

UnityLogFileLocation = os.getenv('localappdata') + '\\Unity\\Editor\\Editor.log'
#UnityLogFileLocation = '{Modest3dDir}/Modest3DLog.txt'

class Platforms:
    Windows = 'windows'
    WebPlayer = 'webplayer'
    Android = 'android'
    WebGl = 'webgl'
    OsX = 'osx'
    Linux = 'linux'
    Ios = 'ios'
    WindowsStoreApp = 'wsa'
    All = [Windows, WebPlayer, Android, WebGl, OsX, Linux, Ios, WindowsStoreApp]

class UnityReturnedErrorCodeException(Exception):
    pass

class UnityUnknownErrorException(Exception):
    pass

class UnityHelper:
    _log = Inject('Logger')
    _sys = Inject('SystemHelper')
    _varMgr = Inject('VarManager')

    def __init__(self):
        pass

    def onUnityLog(self, logStr):
        self._log.debug(logStr)

    def openUnity(self, projectPath, platform):
        self._sys.executeNoWait('"[UnityExePath]" -buildTarget {0} -projectPath "{1}"'.format(self._getBuildTargetArg(platform), projectPath))

    def runEditorFunction(self, projectPath, editorCommand, platform = Platforms.Windows, batchMode = True, quitAfter = True, extraExtraArgs = ''):
        extraArgs = ''

        if quitAfter:
            extraArgs += ' -quit'

        if batchMode:
            extraArgs += ' -batchmode -nographics'

        extraArgs += ' ' + extraExtraArgs

        self.runEditorFunctionRaw(projectPath, editorCommand, platform, extraArgs)

    def _getBuildTargetArg(self, platform):

        if platform == Platforms.Windows:
            return 'win32'

        if platform == Platforms.WebPlayer:
            return 'web'

        if platform == Platforms.Android:
            return 'android'

        if platform == Platforms.WebGl:
            return 'WebGl'

        if platform == Platforms.OsX:
            return 'osx'

        if platform == Platforms.Linux:
            return 'linux'

        if platform == Platforms.Ios:
            return 'ios'

        if platform == Platforms.WindowsStoreApp:
            return 'wsa'

        assertThat(False, "Unhandled platform {0}".format(platform))

    def runEditorFunctionRaw(self, projectPath, editorCommand, platform, extraArgs):

        logPath = self._varMgr.expandPath(UnityLogFileLocation)

        logWatcher = LogWatcher(logPath, self.onUnityLog)
        logWatcher.start()

        assertThat(self._varMgr.hasKey('UnityExePath'), "Could not find path variable 'UnityExePath'")

        try:
            command = '"[UnityExePath]" -buildTarget {0} -projectPath "{1}"'.format(self._getBuildTargetArg(platform), projectPath)

            if editorCommand:
                command += ' -executeMethod ' + editorCommand

            command += ' ' + extraArgs

            self._sys.executeAndWait(command)

        except ProcessErrorCodeException as e:
            raise UnityReturnedErrorCodeException("Error while running Unity!  Command returned with error code.")

        except:
            raise UnityUnknownErrorException("Unknown error occurred while running Unity!")

        finally:
            logWatcher.stop()

            while not logWatcher.isDone:
                time.sleep(0.1)

