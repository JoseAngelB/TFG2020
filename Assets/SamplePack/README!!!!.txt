Leave feedback and see status here https://trello.com/b/1hELo6C9/bolt-sample-pack 

Check out the Beta Samples here before they are added to the main package
https://bit.ly/2WaAz0F

It is recommended to import this project into a new project rather than an existing one, as it currently relies on a few layers and will overwrite your Bolt Assets such as states and events. 
The samples included in the main Photon Bolt package and the ones in this package rely on different Bolt Assets.
You can't have both of them in the same project unless you want to manually add the Bolt Assets from one package to the other.
If you want to add a sample to your existing project, add the Bolt Assets to your existing project, copy the sample folder to your existing project, make sure the entities have the current states selected, and compile assembly. 


Snippets:

Advanced Tutorial Plus: Cannot damage other players through buildings, rocket launcher is now functional and does damage. Clients with latency no longer jitter around on elevator. 
WeaponRifle.cs, WeaponBazooka.cs, PlayerController.cs, PlayerMotor.cs

CommandWorkaround: Prevents server from accepting an excessive amount of commands from server.

HeadlessBuildUtil: Temporarily ignore files for a build for dedicated server headless build.

PhotonSample: Adds new functions to allow joining specific rooms directly. Useful for people using 3rd party matchmaking.
public void CreatePhotonRoom(out string roomName, out string socketPeerId, PhotonOperationFailedDelegate failedDelegate)
public void ConnectViaPhotonRoom(string roomName, string socketPeerId, PhotonOperationFailedDelegate failedDelegate)

vrPlayerController: Simple sample of a VR player syncing head and hands.

BigDataTransfer: Transfer files much larger than the packet size using events.


Version History:

1.0.5 Removed UNET references from Voice Sample
1.0.4 Update to Photon Bolt 1.2.8. Added "README" into each scene. Added link to Beta Samples. (https://bit.ly/2WaAz0F)
1.0.3 Added Authoritative Rigidbody Prediction sample
1.0.2 Updated to Photon Bolt 1.2.5. Added new snippets for transfering large amounts of data using events and extensions for tokens.  
1.0.1 Updated readme with further instructions related to ResourceControl sample. Added HostMigration to the project rather than being a seperate project within a ZIP.
1.0.0 Officially releaseed on the Unity Asset Store



