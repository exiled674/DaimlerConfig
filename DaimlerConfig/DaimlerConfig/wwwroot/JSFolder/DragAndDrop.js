// Generic drag and drop initialization function
window.initializeDragAndDropSafe = function (className, containerSelector) {
    // Clear any existing interactions first
    interact(className).unset();

    // Set grab cursor on all target elements initially
    document.querySelectorAll(className).forEach(el => {
        el.style.cursor = 'grab';
    });

    interact(className)
        .draggable({
            inertia: false,
            modifiers: [
                interact.modifiers.restrictRect({
                    restriction: containerSelector || 'parent',
                    elementRect: { top: 0, left: 0, bottom: 1, right: 1 }
                })
            ],
            autoScroll: true,
            listeners: {
                start(event) {
                    console.log('Drag started:', event.target);

                    // Visual + cursor feedback
                    event.target.classList.add('is-dragging');
                    event.target.style.cursor = 'grabbing';

                    // Store the original position
                    event.target.setAttribute('data-original-transform',
                        event.target.style.transform || 'translate(0px, 0px)');

                    // Get siblings
                    const container = event.target.closest(containerSelector) || event.target.parentNode;
                    const siblings = Array.from(container.querySelectorAll(className))
                        .filter(el => el !== event.target);

                    siblings.forEach(sibling => {
                        const rect = sibling.getBoundingClientRect();
                        sibling.setAttribute('data-original-y', rect.top);
                    });
                },

                move(event) {
                    const target = event.target;
                    const x = (parseFloat(target.getAttribute('data-x')) || 0) + event.dx;
                    const y = (parseFloat(target.getAttribute('data-y')) || 0);

                    // Vertical-only movement
                    target.style.transform = `translate(0px, ${y}px)`;

                    // Store values
                    target.setAttribute('data-x', 0);
                    target.setAttribute('data-y', y);

                    handleReordering(target, event.clientY, className, containerSelector);
                },

                end(event) {
                    console.log('Drag ended:', event.target);

                    const target = event.target;

                    // Visual + cursor reset
                    target.classList.remove('is-dragging');
                    target.style.cursor = 'grab';

                    // Reset transform
                    target.style.transform = '';
                    target.removeAttribute('data-x');
                    target.removeAttribute('data-y');
                    target.removeAttribute('data-original-transform');

                    // Clean up
                    const container = target.closest(containerSelector) || target.parentNode;
                    const siblings = Array.from(container.querySelectorAll(className));

                    siblings.forEach(sibling => {
                        sibling.removeAttribute('data-original-y');
                        sibling.style.transform = '';
                    });

                    // Trigger Blazor callback
                    const elementType = getElementType(target, className);
                    const newOrder = getElementOrder(container, className, elementType);

                    if (window.onElementReordered) {
                        window.onElementReordered(elementType, newOrder);
                    }
                }
            }
        });
};


// Enhanced reordering function that works with any container
function handleReordering(draggedElement, mouseY, className, containerSelector) {
    const container = draggedElement.closest(containerSelector) || draggedElement.parentNode;
    const siblings = Array.from(container.querySelectorAll(className))
        .filter(el => el !== draggedElement);

    // Find the element that should come after the dragged element
    let targetElement = null;
    let insertBefore = true;

    for (let sibling of siblings) {
        const rect = sibling.getBoundingClientRect();
        const siblingMiddle = rect.top + rect.height / 2;

        if (mouseY < siblingMiddle) {
            targetElement = sibling;
            insertBefore = true;
            break;
        }
        targetElement = sibling;
        insertBefore = false;
    }

    // Reorder DOM elements
    if (targetElement) {
        if (insertBefore) {
            container.insertBefore(draggedElement, targetElement);
        } else {
            //Insert after the target element
            if (targetElement.nextSibling) {
                container.insertBefore(draggedElement, targetElement.nextSibling);
            } else {
                container.appendChild(draggedElement);
            }
        }
    }
}

// Determine element type based on class names and structure
function getElementType(element, className) {
    if (element.closest('.sidebar-item') && className.includes('draggable')) {
        return 'station';
    } else if (element.closest('.tool-section') || element.classList.contains('tool-draggable')) {
        return 'tool';
    } else if (element.closest('.operation-section') || element.classList.contains('operation-draggable')) {
        return 'operation';
    }
    return 'unknown';
}

// Get the order of elements based on their type
function getElementOrder(container, className, elementType) {
    const elements = Array.from(container.querySelectorAll(className));

    return elements.map(el => {
        switch (elementType) {
            case 'station':
                const stationLink = el.querySelector('.sidebar-link');
                return stationLink ? stationLink.textContent.trim() : null;

            case 'tool':
                const toolHeader = el.querySelector('.tool-header span');
                return toolHeader ? toolHeader.textContent.trim() : null;

            case 'operation':
                const operationSpan = el.querySelector('.operation-section span');
                return operationSpan ? operationSpan.textContent.trim() : null;

            default:
                return el.textContent.trim();
        }
    }).filter(id => id !== null);
}

// Specific initialization functions for each type
window.initializeStationDragDrop = function () {
    window.initializeDragAndDropSafe('.draggable', '.sidebar-nav');
};

window.initializeToolDragDrop = function () {
    window.initializeDragAndDropSafe('.tool-draggable', '#toolsList');
};

window.initializeOperationDragDrop = function () {
    window.initializeDragAndDropSafe('.operation-draggable', '.operation-list');
};