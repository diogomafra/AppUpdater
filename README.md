AppUpdater
===========

The objective of the AppUpdater component is to allow the application to be constantly and incrementally updated. The behavior is simular to Google Chrome update.

How it works
------------
TO DO

How to use
-----------
TO DO


Application Folder Structure
-----------------------------

- Root
    - AppUpdater.Runner.exe (*application that runs the main application*)
    - config.xml (*contains the version number and the update url*)
    - 2.0.0 (*folder that contains the application*)
        - app.exe
        - library.dll
        - another_library.dll
    - 1.0.0 (*old version of the application*)
        - app.exe
        - library.dll


Server Folder Structure
------------------------------
- Root
    - version.xml (*defines the current version*)
    - 1.0.0
        - app.exe
        - library.dll
        - manifest.xml
    - 2.0.0
        - app.exe
        - library.dll
        - another_library.dll
        - manifest.xml

To Do
-----
- create a tool that generates the manifest and publishes the update;
- test all the common scenarios (e.g. no internet access);
- remove old versions;
- use a diff algorithm to update the files ([Google Courgette](http://dev.chromium.org/developers/design-documents/software-updates-courgette));
- give more info to the application (size of update, update progress, errors...).