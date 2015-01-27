Your Last Options Dialog
========================

Quickstart
----------
1.) Use a custom data container class as foundation for the options dialog. It can have properties of all sorts of types: bool, int, string, datetime etc.
2.) Attribute these properties with the appropriate OptionsXYZ attributes from YLOD to determine the looks, localization, confirmation dialogs, validation logic etc.
3.) In your application, use the following line of code to navigate to the options screen: OptionsService.Current.Show(options); ... where "options" is an instance of your data container

More
----
Sample walkthrough: http://ylod.codeplex.com/wikipage?title=SampleWalkthrough&referringTitle=Documentation

YLOD supports loads of features:

* Many built-in data types: http://ylod.codeplex.com/wikipage?title=SupportedDataTypes&referringTitle=Documentation
* Additional data types (colors, timespans etc.) in the "YLOD.Extras" package
* Restore after tombstoning: http://ylod.codeplex.com/wikipage?title=Tombstoning&referringTitle=Documentation
* Localization: http://ylod.codeplex.com/wikipage?title=Localization&referringTitle=Documentation
* Providing your own views/editors and validators: http://ylod.codeplex.com/wikipage?title=CustomViews&referringTitle=Documentation
* Providing your own data types: http://ylod.codeplex.com/wikipage?title=CustomDataTypes&referringTitle=Documentation
* ...

To learn more about these features, please take a look at the official documentation:

http://ylod.codeplex.com/documentation

For feature requests or bug reports also use the project page:

http://ylod.codeplex.com

If you think YLOD is useful, please leave a vote or tweet about it - thanks :)

--
Peter Kuhn ("Mister Goodcat")
Twitter: @Mister_Goodcat (https://twitter.com/Mister_Goodcat)
Blog: http://www.pitorque.de/MisterGoodcat