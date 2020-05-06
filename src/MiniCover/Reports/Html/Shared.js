function hoverTargetHandler(e) {
    if (e.target.dataset.hoverTarget) {
        let elements = document.querySelectorAll(e.target.dataset.hoverTarget);
        if (e.type == 'mouseover')
            elements.forEach(x => x.classList.add('hover'));
        else if (e.type == 'mouseout')
            elements.forEach(x => x.classList.remove('hover'));
    }
}
document.addEventListener('mouseover', hoverTargetHandler);
document.addEventListener('mouseout', hoverTargetHandler);

function activateTargetHandler(e) {
    document.querySelectorAll('.active')
        .forEach(x => x.classList.remove('active'));

    if (e.target.dataset.activateTarget) {
        let elements = document.querySelectorAll(e.target.dataset.activateTarget);
        elements.forEach(x => x.classList.add('active'));
    }
}
document.addEventListener('click', activateTargetHandler);

function toggleTargetHandler(e) {
    if (e.target.dataset.toggleTarget) {
        let elements = document.querySelectorAll(e.target.dataset.toggleTarget);
        elements.forEach(x => {
            if (x.classList.contains(show)) {
                x.classList.remove('show');
                x.classList.add('hide');
            } else {
                x.classList.remove('hide');
                x.classList.add('show');
            }
        });
    }
}
document.addEventListener('click', toggleTargetHandler);
