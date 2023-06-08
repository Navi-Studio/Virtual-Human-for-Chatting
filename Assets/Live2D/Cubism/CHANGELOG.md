# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).


## [4-r.6.2] - 2023-03-16

### Fixed

* Fix some problems related to Cubism Core.
  * See `CHANGELOG.md` in Core.


## [4-r.6.1] - 2023-03-10

### Added

* Add function to validate MOC3 files.


## [4-r.6] - 2023-01-26

### Added

* Add the feature to search for Subrig objects by the name of the physics group. by [@ppcuni](https://github.com/ppcuni)
* Add the feature to set the magnification of the output of the physics group uniformly to an arbitrary value. by [@ppcuni](https://github.com/ppcuni)
* Add the feature to uniformly set the inversion flag for the output of the physics group. by [@ppcuni](https://github.com/ppcuni)
* Add editor extension to the Inspector of CubismPhysicsController to display physics group names. by [@ppcuni](https://github.com/ppcuni)
  * To apply this feature to sample models, we regenerated sample models prefab, etc.

### Changed

* Sample model assets are brought up to date.
* Change the version of the development project to `2020.3.41f1`.

### Fixed

* Fix a bug that caused a warning to appear when importing a model with an unmatched Object Name.
* Fix an issue where unwanted curves on `.anim` were not correctly erased when re-importing a model where Pose3.json exists.
* Fix wrong condition for getter in HandlerInterface of Multiply Color and Screen Color. by [@qualice-hirakawa](https://github.com/qualice-hirakawa)
* After setting the multiply color (or screen color) and related flags on the inspector, the changes are now retained when starting play mode. by [@qualice-hirakawa](https://github.com/qualice-hirakawa)
* Fix a bug that caused `.fadeMotionList` to be created incorrectly when loading a model containing `.pose3.json`, depending on the order in which the `CubismPoseMotionImporter` was executed.


## [4-r.5] - 2022-09-08

### Added

* Add Unity 2022 to the development environment.
* Add the multilingual supported documents.
* Add immediate stabilization of physics.

### Removed

* Remove Unity 2019 from the development environment.


## [4-r.5-beta.5] - 2022-08-04

### Added

* Add a feature to change the value of components in batches.

### Changed

* Update `Mao` model.

### Fixed

* Fix crash with exception when reading .moc3 files of unsupported versions.
  * The console now produces an explicit error log instead of an exception.
* Fix physics system input to be split by the physics setting time.
* Fix a bug in which processing was interrupted when an invalid vertex was specified in the middle of a physics operation.


## [4-r.5-beta.4] - 2022-07-07

### Added

* Add properties to get the latest .moc3 version and the .moc3 version of the loaded model in CubismMoc.
* Add `Mao` model.
* Add `Demo4.2` scene to run `Mao` model.
* Add a function to get the type of parameters of the model.
* Add a function to get the parent part of the model's Drawable.

### Fixed

* Fix `GetMocVersion` function argument in `CoreDll` class that was different from the original Core API argument.
* Fix that does not occur error when Sorting Mode of CubismRenderController is changed in the Project window.


## [4-r.5-beta.3] - 2022-06-16

### Changed

* Change the version of the development project to `2019.4.39f1`.

### Fixed

* Fix physics system behaviour when exists Physics Fps Setting in .physics3.json.


## [4-r.5-beta.2] - 2022-06-02

### Fixed

* Fixed a bug that caused Multiply Color / Screen Color of different objects to be applied.
  * See `CHANGELOG.md` in Core.
  * No modifications to Samples and Framework.


## [4-r.5-beta.1] - 2022-05-19

### Added

* Add support to prevent exceptions depending on the presence or absence of parameters. by [@ppcuni](https://github.com/ppcuni)
* Add Weight argument to BlendToValue function. by [@ppcuni](https://github.com/ppcuni)
* Add processing related to Multiply Color / Screen Color added in Cubism 4.2.

### Fixed

* Fix for changes `EditorUserBuildSettings` in 2021.2 and later.


## [4-r.4.2] - 2022-03-09

### Fixed

* Fix an issue where inversion masks did not work properly and the mask limit was different from normal.


## [4-r.4.1] - 2022-02-10

### Added

* Added an editor extension that switches the library depending on Unity version when exporting to WebGL.


## [4-r.4] - 2021-12-09

### Added

* Add Unity 2021 to the development environment.
* Add the function to set assets to a component when importing a model.
* Add Chrome OS from the tested environment.

### Changed

* Change the version of the development project to `2019.4.29f1`.
* Change `Enable Async` in `AsyncBenchmark` scene to be disabled by default.
* Change the sample scene in `Samples/OW/Expression` so that the expressions are displayed in the order of the elements in `expressionList.asset`.
* Change `UserData` that it can be edited from Inspector.
* Change multiple `UserData` can be edited in bulk from the inspector.

### Fixed

* Fix an issue with duplicate elements in the Expression List.
* Fix the elements of `expressionList.asset` to be empty when importing in Unity 2021 and 2020.
* Fix to keep `Layer` at reimport.
* Fix `CubismMotionController` do not be removed at reimport.

### Removed

* Remove Unity 2018 from the development environment.


## [4-r.3] - 2021-06-10

### Added

* Add a function to read data from `.cdi3.json`. by [@ShigemoriHakura](https://github.com/ShigemoriHakura)
* Add a function to display the names of parameters and parts described in `.cdi3.json`. by [@ShigemoriHakura](https://github.com/ShigemoriHakura)
* Add a function to change the display name of parameters and parts to any name.
* Add a display of elapsed time to the sample scene `AsyncBenchmark`.
* Add a sample scene that manipulates the number of models to be displayed to achieve the specified frame rate.

### Changed

* Change to continue importing when an error occurs. [@TakahiroSato](https://github.com/TakahiroSato)

### Fixed

* Fix failing the model generation when importing with Unity 2020.
* Fix registering the dynamically selection of the index for `PlayerLoopSystem.subSystemList`.
* Fix the updating process for InstanceId; Update the InstanceId without adding the element of .fadeMotionList when the InstanceId that is registered the AnimationClip was changed.
* Fix import of models with invalid masks. by [@DenchiSoft](https://github.com/DenchiSoft), [@ShigemoriHakura](https://github.com/ShigemoriHakura).


## [4-r.2] - 2021-01-12

### Added

* Add Unity 2020 to the development environment.
* Add a process that applies the Culling configuration from Model Data.
* Add an `AnimatorController` generator that is already set `CubismFadeStateObserver`.

### Changed

* Change the setting of Player Loop customized by other assets not to be rewritten in Unity 2019.3 or later.
* Change to get the path of the audio files associated with `.motion3.json` from `.model3.json`.
* Change to enable the appropriate Cubism iOS plugin before building.
* Change the handling of `UnmanagedArrayView` pointers due to improve the performance. [@ppcuni](https://github.com/ppcuni)
* Change the default importing mode to `Original Workflow`.
  * Note: Importing this version into an older version SDK will override this setting.
    see [Cubism SDK Manual page](https://docs.live2d.com/cubism-sdk-manual/unity-for-ow/)

### Fixed

* Fix the bug of the weight calculation of `MotionFade`.
* Fix the bug of the weight calculation of `Expression`.
* Fix the bug when playing a motion that is shorter than the fade time or transitioning to a motion with a different fade value.
* Fix fixed value for the generated .exp3.asset fade value when there is no fade value in .exp3.json.
* Fix registering the delegate duplicately on calling `CubismUpdateController.Refresh()`.
* Fix checking bounding box hit every time before checking mesh hit by [@DenchiSoft](https://github.com/Live2D/CubismUnityComponents/pull/42).

### Removed

* Remove Unity 2017 from the development environment.


## [4-r.1] - 2020-01-30

### Changed

* Change registration of `CubismModel.OnRenderObject()` to `Player Loop` on Unity 2018.
* Change access modifier of `CubismFadeController.Refresh()` to `public`.
* Restructure scene maintaining Unity 2017-2019 compatibility.
* Change to manage .meta files, samples and models in the repository.
* Separate license file from `README.md`.
* Change to use gravity and wind settings from `physics3.json`.
* Reformat some code according to the coding standards.
* Change the shorten type name `Action` to full name `System.Action`.

### Fixed

* Fix occurring runtime error when importing the model exist with `.moc3.asset`.
* Fix only `EndTime` update with latest motion when motion fading.
* Fix the path of `.motion3.json` to save `.fade.asset`.
* Fix the `InstanceId`.
* Fix to reuse `InstanceId` recorded in generated `AnimationClip`.
* Fix condition to clear existing `AnimationClip` curve on reimporting.
* Fix the script of the `Original Workflow sample` scene.


## [4-beta.2] - 2019-11-14

### Fixed

* Fix sample scene sourcecode: `CubismSampleController.cs`
* Fix a bug when switching multiple motions in the same layer of `Motion` component.
* Fix priority not being set when playing idle motion.
* Fix importing process of `.pose3.json` on `Original Workflow`.


## [4-beta.1] - 2019-09-04

### Added

* Support new Inverted Masking features.
* Add `.editorconfig` and `.gitattributes` to manage file formats.
* Add `UWP/ARM64` Core binary file for experimental use.
* Add method to know playing motion or not [#35](https://github.com/Live2D/CubismUnityComponents/pull/35).
* Add sample model and sample scene.(`./Assets/Live2D/Cubism/Samples/OriginalWorkflow/DemoCubism4`)


### Changed

* Upgrade Core version to 04.00.0000 (67108864).
* Move to `Plugin/Experimental/UWP` from `Plugin/Experimental/uwp/Windows`.
* Convert all file formats according to `.editorconfig`.
* `LICENSE.txt` file has been integrated into `README.md`
* Remove changelog and regenerate `CHANGELOG.md`.
* What was `Package.json` is currently being changed to`cubism-info.yml`.
* Improve CubismUpdateController [#34](https://github.com/Live2D/CubismUnityComponents/pull/34).

### Fixed

* Fix issue of `Demo` and `Motion` sample in `OriginalWorkflow`.
* Fix issue that mesh remain when deleting model.
* Fix issue where Priority value was not reset after playing motion with CubismMotionController.


[4-r.6.2]: https://github.com/Live2D/CubismUnityComponents/compare/4-r.6.1...4-r.6.2
[4-r.6.1]: https://github.com/Live2D/CubismUnityComponents/compare/4-r.6...4-r.6.1
[4-r.6]: https://github.com/Live2D/CubismUnityComponents/compare/4-r.5...4-r.6
[4-r.5]: https://github.com/Live2D/CubismUnityComponents/compare/4-r.5-beta.5...4-r.5
[4-r.5-beta.5]: https://github.com/Live2D/CubismUnityComponents/compare/4-r.5-beta.4...4-r.5-beta.5
[4-r.5-beta.4]: https://github.com/Live2D/CubismUnityComponents/compare/4-r.5-beta.3...4-r.5-beta.4
[4-r.5-beta.3]: https://github.com/Live2D/CubismUnityComponents/compare/4-r.5-beta.2...4-r.5-beta.3
[4-r.5-beta.2]: https://github.com/Live2D/CubismUnityComponents/compare/4-r.5-beta.1...4-r.5-beta.2
[4-r.5-beta.1]: https://github.com/Live2D/CubismUnityComponents/compare/4-r.4.2...4-r.5-beta.1
[4-r.4.2]: https://github.com/Live2D/CubismUnityComponents/compare/4-r.4.1...4-r.4.2
[4-r.4.1]: https://github.com/Live2D/CubismUnityComponents/compare/4-r.4...4-r.4.1
[4-r.4]: https://github.com/Live2D/CubismUnityComponents/compare/4-r.3...4-r.4
[4-r.3]: https://github.com/Live2D/CubismUnityComponents/compare/4-r.2...4-r.3
[4-r.2]: https://github.com/Live2D/CubismUnityComponents/compare/4-r.1...4-r.2
[4-r.1]: https://github.com/Live2D/CubismUnityComponents/compare/4-beta.2...4-r.1
[4-beta.2]: https://github.com/Live2D/CubismUnityComponents/compare/4-beta.1...4-beta.2
[4-beta.1]: https://github.com/Live2D/CubismUnityComponents/compare/86e5b07702f74d00b4ab52b7d6c15ba3464b8b85...4-beta.1
