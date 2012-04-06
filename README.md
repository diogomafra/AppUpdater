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
        - app.exe.deploy
        - library.dll.deploy
        - manifest.xml
    - 2.0.0
        - app.exe.deploy
        - library.dll.deploy
        - another_library.dll.deploy
        - manifest.xml

To Do
-----
- test all the common scenarios (e.g. no internet access);
- remove old versions;
- use a diff algorithm to update the files ([MSDelta](http://msdn.microsoft.com/en-us/library/bb417345.aspx#msdelta));
- give more info to the application (size of update, update progress, errors...);
- validate the manifest and config before reading them.
