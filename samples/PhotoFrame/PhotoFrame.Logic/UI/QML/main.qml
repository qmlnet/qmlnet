import QtQuick 2.7
import QtQuick.Controls 2.0
import QtQuick.Layouts 1.0
import QtGraphicalEffects 1.0
import app 1.1

//import QtTest 1.0

ApplicationWindow {
    id: window
    visible: true
    width: 640
    height: 480
    title: qsTr("PhotoFrame")

//    TestEvent {
//        id: test
//    }

//    function delay_ms(delay_time) {
//        test.mouseClick(window, 0, 0, Qt.NoButton, Qt.NoModifier, delay_time)
//    }

	//trigger javascript gc periodically to get a better view on memory consumption
//	Timer {
//		interval: 1000;
//		running: true;
//		repeat: true
//		onTriggered: {
//			gc()
//		}
//	}

    ParallelAnimation {
        id: animMoveUp
        PropertyAnimation { target: animator.secondaryLoader; properties: "y"; to: -height; duration: appModel.animationDurationMs }
        PropertyAnimation { target: animator.mainLoader; properties: "y"; to: 0; duration: appModel.animationDurationMs }
        onRunningChanged: {
            if(!running) {
                animator.endAnimation(animMoveUp);
            }
        }
    }

    ParallelAnimation {
        id: animMoveRight
        PropertyAnimation { target: animator.secondaryLoader; properties: "x"; to: width; duration: appModel.animationDurationMs }
        PropertyAnimation { target: animator.mainLoader; properties: "x"; to: 0; duration: appModel.animationDurationMs }
        onRunningChanged: {
            if(!running) {
                animator.endAnimation(animMoveRight);
            }
        }
    }

    ParallelAnimation {
        id: animMoveRightAndRotate
        PropertyAnimation { target: animator.secondaryLoader; properties: "x"; to: width; duration: appModel.animationDurationMs }
        PropertyAnimation { target: animator.mainLoader; properties: "x"; to: 0; duration: appModel.animationDurationMs }
        RotationAnimation { targets: [animator.secondaryLoader, animator.mainLoader]; from: 0; to: 360; duration: appModel.animationDurationMs }
        onRunningChanged: {
            if(!running) {
                animator.endAnimation(animMoveRightAndRotate);
            }
        }
    }

    SequentialAnimation {
        id: animFade
        NumberAnimation { target: animator.secondaryLoader; properties: "opacity"; to: 0; duration: appModel.animationDurationMs / 2 }
        NumberAnimation { target: animator.mainLoader; properties: "opacity"; to: 1; duration: appModel.animationDurationMs / 2 }
        onRunningChanged: {
            if(!running) {
                animator.endAnimation(animFade);
            }
        }
    }

    SequentialAnimation {
        id: animScale
        NumberAnimation { target: animator.secondaryLoader; properties: "scale"; to: 0; duration: appModel.animationDurationMs / 2 }
        NumberAnimation { target: animator.mainLoader; properties: "scale"; to: 1; duration: appModel.animationDurationMs / 2 }
        onRunningChanged: {
            if(!running) {
                animator.endAnimation(animScale);
            }
        }
    }

    QtObject {
        id: animator
        property var mainLoader: loader1;
        property var secondaryLoader: loader2;

		property var currentAnimation: null;

        function switchLoader()
        {
            var tmp = mainLoader;
            mainLoader = secondaryLoader;
            secondaryLoader = tmp;
        }

		function cancelAllAnimations()
		{
			animMoveUp.complete();
			animMoveRight.complete();
			animMoveRightAndRotate.complete();
			animFade.complete();
			animScale.complete();
		}

        function initAnimation(animation)
        {
			currentAnimation = animation;
			cancelAllAnimations();
            switchLoader();
            mainLoader.y = 0;
            mainLoader.x = 0;
            secondaryLoader.y = 0;
            secondaryLoader.x = 0;

            mainLoader.opacity = 1;
            secondaryLoader.opacity = 1;

            mainLoader.scale = 1;
            secondaryLoader.scale = 1;

            mainLoader.visible = true;
            secondaryLoader.visible = true;
        }

        function endAnimation(animation) {
			if(currentAnimation == animation)
			{
				secondaryLoader.visible = false;
				currentAnimation = null;
			}
        }

        function switchViewFade(resourceId, viewModel) {
            initAnimation(animFade);
            secondaryLoader.opacity = 1;
            mainLoader.opacity = 0;
            setViewToLoader(mainLoader, resourceId, viewModel);

            animFade.start();
        }

        function switchViewVertical(resourceId, viewModel) {
            initAnimation(animMoveUp);
            secondaryLoader.y = 0;
            mainLoader.y = height;

            setViewToLoader(mainLoader, resourceId, viewModel);

            animMoveUp.start();
        }

        function switchViewHorizontal(resourceId, viewModel) {
            initAnimation(animMoveRight);
            secondaryLoader.x = 0;
            mainLoader.x = -width;

            setViewToLoader(mainLoader, resourceId, viewModel);

            animMoveRight.start();
        }

        function switchViewScale(resourceId, viewModel) {
            initAnimation(animScale);
            secondaryLoader.scale = 1;
            mainLoader.scale = 0;

            setViewToLoader(mainLoader, resourceId, viewModel);

            animScale.start();
        }

        function switchViewRotateAndMove(resourceId, viewModel) {
            initAnimation(animMoveRightAndRotate);
            secondaryLoader.x = 0;
            mainLoader.x = -width;

            setViewToLoader(mainLoader, resourceId, viewModel);

            animMoveRightAndRotate.start();
        }

        function switchViewHard(resourceId, viewModel) {
            initAnimation();
            setViewToLoader(mainLoader, resourceId, viewModel);
        }

        function setViewToLoader(loader, resourceId, viewModel) {
            loader.setSource(resourceId, { "viewModel": viewModel })
            //gc is called here to better watch memory consumption.
            //this place doesn't influence any animations as the view switch animation will be
            //triggered shortly after that call
            gc();
        }
    }

	function updateViewData(){
        if(animator.mainLoader.source !== appModel.currentViewSwitchInfo.viewResourceId) {
            var switchType = appModel.currentViewSwitchInfo.switchTypeString;
            switch(switchType) {
            case "Fade":
                animator.switchViewFade(appModel.currentViewSwitchInfo.viewResourceId, appModel.currentViewSwitchInfo.viewModel);
                break;
            case "Vertical":
                animator.switchViewVertical(appModel.currentViewSwitchInfo.viewResourceId, appModel.currentViewSwitchInfo.viewModel);
                break;
            case "Horizontal":
                animator.switchViewHorizontal(appModel.currentViewSwitchInfo.viewResourceId, appModel.currentViewSwitchInfo.viewModel);
                break;
            case "RotateAndMove":
                animator.switchViewRotateAndMove(appModel.currentViewSwitchInfo.viewResourceId, appModel.currentViewSwitchInfo.viewModel);
                break;
            case "Scale":
                animator.switchViewScale(appModel.currentViewSwitchInfo.viewResourceId, appModel.currentViewSwitchInfo.viewModel);
                break;
            case "None":
                animator.switchViewHard(appModel.currentViewSwitchInfo.viewResourceId, appModel.currentViewSwitchInfo.viewModel);
                break;
            default:
                animator.switchViewHard(appModel.currentViewSwitchInfo.viewResourceId, appModel.currentViewSwitchInfo.viewModel);
                break;
            }
        }
	}

    AppModel {
		id: appModel
        Component.onCompleted: {
            appModel.currentViewSwitchInfoChanged.connect(updateViewData)
        }
    }

    Loader {
        id: loader1
        y: 0
        height: parent.height
        width: parent.width
        x: 0
        Component.onCompleted: {
			updateViewData();
		}
    }

    Loader {
        id: loader2
        y: 0
        height: parent.height
        width: parent.width
        x: 0
        visible: false
    }

    Item
    {
        y: 0
        height: parent.height
        width: parent.width
        x: 0
		visible: appModel.showDebugInfo

		Rectangle {
             id: viewName
             anchors.left: parent.left
             anchors.top: parent.top
             anchors.margins: 10
             width: 80
             height: 30
             color: "darkgrey"
             radius: width * 0.1
             Text {
                 anchors.centerIn: parent
                 id: txtViewName
                 text: appModel.currentViewName
                 font.pointSize: 8
                 color: "white"
             }
        }
        DropShadow {
            anchors.fill: viewName
            cached: true
            horizontalOffset: 3
            verticalOffset: 3
            radius: 8.0
            samples: 17
            color: "#80000000"
            source: viewName
        }

		Rectangle {
             id: switchType
             anchors.left: parent.left
             anchors.bottom: parent.bottom
             anchors.margins: 10
             width: 80
             height: 30
             color: "darkgrey"
             radius: width * 0.1
             Text {
                 anchors.centerIn: parent
                 id: txtSwitchType
                 text: appModel.currentSwitchTypeName
                 font.pointSize: 8
                 color: "white"
             }
        }
        DropShadow {
            anchors.fill: switchType
            cached: true
            horizontalOffset: 3
            verticalOffset: 3
            radius: 8.0
            samples: 17
            color: "#80000000"
            source: switchType
        }

		Rectangle {
            id: memoryUsage
             anchors.right: parent.right
             anchors.top: parent.top
             anchors.margins: 10
             width: 70
             height: 30
             color: "darkgrey"
             radius: width * 0.2
             Text {
                 anchors.centerIn: parent
                 id: txtMemoryUsage
                 text: appModel.currentlyUsedMBString
                 font.pointSize: 10
                 color: "white"
             }
        }
        DropShadow {
            anchors.fill: memoryUsage
            cached: true
            horizontalOffset: 3
            verticalOffset: 3
            radius: 8.0
            samples: 17
            color: "#80000000"
            source: memoryUsage
        }

        Rectangle {
            id: circleCounter
             anchors.right: parent.right
             anchors.bottom: parent.bottom
             anchors.margins: 10
             width: 40
             height: width
             color: "darkgrey"
             radius: width * 0.5
             Text {
                 anchors.centerIn: parent
                 id: txtCountDown
                 text: appModel.timerValue
                 font.pointSize: 16
                 color: "white"
             }
        }
        DropShadow {
            anchors.fill: circleCounter
            cached: true
            horizontalOffset: 3
            verticalOffset: 3
            radius: 8.0
            samples: 17
            color: "#80000000"
            source: circleCounter
        }
    }
}
