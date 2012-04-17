AppUpdater
===========

The objective of the AppUpdater component is to allow applications to be constantly and incrementally updated without user intervention.
The behavior is similar to Google Chrome's update. Each update downloads the minimum possible amount of data.


How it works
------------
1. The application checks from time to time the version defined in the update server;
2. If the version is different from the one installed, then it starts the update procedure;
3. A new folder is created for the new version, the unaltered files are copied to the new folder and the changed ones are downloaded;
4. If the changed file has a delta available in the update server, only the delta is downloaded and not the entire file;
5. The new version is set as the "current version";
6. The next time the app is started it will use the new version.


How to use it
-------------
To use the AppUpdater library the application must:

1. follow a pre-defined structure (see later);
2. have an update server with a pre-defined structure (see later);
3. activate the code that checks for updates (see later).


Client directory structure
--------------------------
The application that uses the AppUpdater component must have a pre-defined structure.
The root of the application must contain the *Runner* and the *config.xml* file.
The application must be inside a folder with the same name as the current version.

The application is not directly executed. The *Runner* is executed and executes the corresponding application based on the current version.
The *config.xml* file has the definition of the current version and the url of the update server.

Client structure:

* Root
    * AppUpdater.Runner.exe
	* config.xml
	* 1.0.0
		* MyApp.exe
		* MyLib.dll


Server directory structure
--------------------------
The update files must be available in a web server, following a pre-defined structure.
You don't need to create this structure manually, it's created by the publisher (see later).
The update server url must be defined in the *config.xml* file of the client app.

The *version.xml* file defines the current version. Based on this information, the corresponding directory is used.
Inside each directory there is a manifest file and the files that must be deployed. The manifest file defines the files to be downloaded.

Each version can contain deltas that are used to update without downloading the entire file, just the parts that were changed.

Server structure:

* Root
	* version.xml
	* 1.0.0
		* manifest.xml
		* MyApp.exe.deploy
		* MyLib.dll.deploy
	* 2.0.0
		* manifest.xml
		* MyApp.exe.deploy
		* MyLib.dll.deploy
		* deltas
			* MyApp.exe.F872A.deploy
			* MyLib.dll.B89C0.deploy


Application Code
----------------
You can use the following code to check for updates every hour. The update procedure will be executed automatically.

```csharp
AutoUpdater autoUpdater = new AutoUpdater(UpdateManager.Default);
autoUpdater.SecondsBetweenChecks = 3600;
autoUpdater.Start();
```
You can also subscribe to the event `Updated` in order to be notified when an update has been made.


Update process and deltas
-------------------------
The update process will download only the new files and those which were changed.
The files that weren't changed between versions will just be copied from the old version.

If a file was modified, the update process will check if there is a delta available. 
If there is one, it will use the delta instead of downloading the entire file.

In a normal update, only new files and the deltas will be downloaded. That means the update size will be very small.


Publishing a version
--------------------
To publish a version to the update server you must use the **AppUpdater.Publisher** utility. 
It publishes the files to a defined directory; this directory must be served by a web server. 
Right now, the utility can't publish directly to a web server, just to a directory.

The usage of the publisher is as follow:

    AppUpdate.Publisher.exe -source:source_dir -target:target_dir -deltas:2 -version:1.0.0

The **"source"** argument defines the directory containing the new version files. The **"target"** defines the directory of the update server. 
And **"version"** defines the number of the new version. 
The **"deltas"** argument is optional; if defined, the publisher will generate deltas for older versions. The number of old versions is defined in this argument.

If, for example, you define the **"deltas"** argument as **"10"**, the publisher will generate deltas for the last 10 versions. 
It means that if a client is 10 versions behind the current one, it can use the delta instead of downloading the entire file.

If the application is constantly executed, the user will probably have the latest version. 
If it is used less frequently, it can have an older version, in that case it's a good idea to generate deltas for as many versions as you think would be needed.


Limitations
-----------
Right now, the application can't be installed in the *"Program Files"* directory. 
Applications don't have permissions to write to the *"Program Files"* directory, which would keep the update process from being executed.


Creating a setup
----------------
To create a setup:

1. do not install on the *Program Files* folder (see above);
2. define the version as 0.0.0 (or something like that) in the *config.xml*;
3. install the app in the folder corresponding to the version (in this example, it would be 0.0.0);
4. define the update server url in the *config.xml*.

There is no problem in defining a version that doesn't exist. 
On the first run, the application will check for updates, will see a different version number and execute the update procedure. 
If the files in the latest version are equals to the files in the setup, they will just be copied to the new folder. No files will be downloaded.