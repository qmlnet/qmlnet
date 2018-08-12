# Qml.Net - PhotoFrame sample

## Purpose

This sample is intended as a "real world" showcase for Qml.Net.

It has been created as a little PoC showcase for geting .Net and Qml running on an embedded Linux platform and to show how easy it is to implement such an application using .Net (in contrast to C++)

"real world" is written in quotes because this code never has been inteded to run on any kind of product but it tries to provide a little more complex use cases than a typical hello world application.

## Basic functionality

The PhotoFrame sample is able to show images from a configured images path.
It provides hot reload of manipulated images in the images directory (add / remove image files) and the configuration file.
It currently iterates through all images from the images directory and starts over when every image has been shown.
To show the images PhotoFrame uses one of the Views randomly and shows a random animation between the views.
- Normal
- Border
- Colorized

## Structure

This project has been inteded to showcase some advantages of .Net / Qml over C++ / Qml.
This is why this application contains some features and a structure that seems a bit overcomplicated :)

### PhotoFrame.App
This project only pulls everything together and provides the initial configuration

### PhotoFrame.Logic
Contains the application logic and views. The application is basically divided into
- UI
- Business Logic
- Configuration

### PhotoFrame.Tests
Contains unit tests for the logic

## QML.Net specialties
The approach of PhotoFrame is to have as less Qml specifics in the code as possible.

To achieve this the MvvmBehavior of Qml.Net is used to support the usage of .Net typical ViewModels implementing the INotifyPropertyChanged interface.

The communication channel between the .Net world and the Qml world is the AppModel which is registered as a Qml type.
The AppModel controls what view should be active, what viewModel should be used and what the view switch animation shall be.

It also contains all the configuration aspects relevant for the Qml world.

The view switching logic and animation control happens completely in the Qml world (main.qml) based on the data that is provided by the AppModel.

The .Net world prepares all the data needed, fires the event (PropertyChanged) and the Qml world listens to the signal fired and starts the animation to the new view.

## How it works
Switching the views is controlled by the FrameController.

The FrameController has the switch timer and every time it ticks it prepares all new parameters and fires a photo changed event which triggers the AppModel to switch to a new View which leads to the Qml world doing its thing.

Whenever the configuration is changed or the images get manipulated the whole process starts over.

The image directory and config watching happens by using a FileSystemWatcher. This has been done to show how easy such a functionality can be implemented in .Net - in contrast to C++.

