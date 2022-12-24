window.startKeyMonitoring = (objectReference) => {
    document.onkeydown = (e) => {
        e = e || window.event;
        if (e.keyCode === 38) {
            objectReference.invokeMethod('KeyHandler', 'up');
        } else if (e.keyCode === 40) {
            objectReference.invokeMethod('KeyHandler', 'down');
        } else if (e.keyCode === 37) {
            objectReference.invokeMethod('KeyHandler', 'left');
        } else if (e.keyCode === 39) {
            objectReference.invokeMethod('KeyHandler', 'right');
        }
    }
}