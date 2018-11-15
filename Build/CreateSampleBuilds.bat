@echo off

set PYTHONPATH=%~dp0/python;%PYTHONPATH%
python -m mtm.zen.CreateSampleBuilds %*

