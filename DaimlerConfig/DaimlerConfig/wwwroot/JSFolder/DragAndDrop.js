

window.initializeDragAndDropSafe = function (className) 
    {
    interact(className).unset();

    interact(className)
        .draggable({
            inertia: false,
            modifiers: [
                // Restrict movement to the sidebar
                interact.modifiers.restrictRect({
                    restriction: '#sidebar',
                    elementRect: { top: 0, left: 0, bottom: 1, right: 1 }
                })
            ],
            autoScroll: true,
            listeners: {
                start(event) {
                    console.log('Drag started:', event.target);

                    // Add visual feedback
                    event.target.classList.add('is-dragging');

                    // Store the original position
                    event.target.setAttribute('data-original-transform',
                        event.target.style.transform || 'translate(0px, 0px)');

                    // Get all draggable siblings for reordering
                    const siblings = Array.from(event.target.parentNode.children)
                        .filter(el => el.classList.contains('draggable') && el !== event.target);

                    // Store original positions of all siblings
                    siblings.forEach(sibling => {
                        const rect = sibling.getBoundingClientRect();
                        sibling.setAttribute('data-original-y', rect.top);
                    });
                },

                move(event) {
                    const target = event.target;
                    const x = (parseFloat(target.getAttribute('data-x')) || 0) + event.dx;
                    const y = (parseFloat(target.getAttribute('data-y')) || 0) + event.dy;

                    // Only allow vertical movement (comment out the x = 0 line if you want horizontal movement too)
                    // x = 0; // Uncomment this line to restrict to vertical-only movement

                    // Apply transform
                    target.style.transform = `translate(${x}px, ${y}px)`;

                    // Store position in data attributes
                    target.setAttribute('data-x', x);
                    target.setAttribute('data-y', y);

                    // Handle reordering of siblings
                    handleReordering(target, event.clientY);
                },

                end(event) {
                    console.log('Drag ended:', event.target);

                    const target = event.target;

                    // Remove visual feedback
                    target.classList.remove('is-dragging');

                    // Reset position
                    target.style.transform = '';
                    target.removeAttribute('data-x');
                    target.removeAttribute('data-y');
                    target.removeAttribute('data-original-transform');

                    // Clean up all siblings
                    const siblings = Array.from(target.parentNode.children)
                        .filter(el => el.classList.contains('draggable'));

                    siblings.forEach(sibling => {
                        sibling.removeAttribute('data-original-y');
                        sibling.style.transform = '';
                    });

                    // Optional: Call a callback to notify the Blazor component about the new order
                    if (window.onStationReordered) {
                        const newOrder = getStationOrder();
                        window.onStationReordered(newOrder);
                    }
                }
            }
        });
};

function handleReordering(draggedElement, mouseY) {
    const container = draggedElement.parentNode;
    const siblings = Array.from(container.children)
        .filter(el => el.classList.contains('draggable') && el !== draggedElement);

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
            // Insert after the target element
            if (targetElement.nextSibling) {
                container.insertBefore(draggedElement, targetElement.nextSibling);
            } else {
                container.appendChild(draggedElement);
            }
        }
    }
}

function getStationOrder() {
    const container = document.querySelector('.sidebar-nav');
    const stationElements = Array.from(container.querySelectorAll('.draggable'));

    // Extract station IDs or names from the elements
    return stationElements.map(el => {
        // Try to get station ID from data attribute or extract from content
        const stationLink = el.querySelector('.sidebar-link');
        if (stationLink) {
            // You might need to adjust this based on how you store station identifiers
            const stationText = stationLink.textContent.trim();
            return stationText;
        }
        return null;
    }).filter(id => id !== null);
}

// Enhanced setup function (keeping your existing one as backup)
function setupDragDropEnhanced() {
    const draggables = document.querySelectorAll('.draggable');
    const container = document.querySelector('.sidebar-nav');

    if (!container) return;

    draggables.forEach(draggable => {
        draggable.draggable = true;

        draggable.addEventListener('dragstart', (e) => {
            draggable.classList.add('dragging');
            // Store the dragged element reference
            e.dataTransfer.effectAllowed = 'move';
            e.dataTransfer.setData('text/html', draggable.outerHTML);
        });

        draggable.addEventListener('dragend', () => {
            draggable.classList.remove('dragging');
        });
    });

    container.addEventListener('dragover', (e) => {
        e.preventDefault();
        e.dataTransfer.dropEffect = 'move';

        const afterElement = getDragAfterElement(container, e.clientY);
        const draggable = document.querySelector('.dragging');

        if (draggable) {
            if (afterElement == null) {
                container.appendChild(draggable);
            } else {
                container.insertBefore(draggable, afterElement);
            }
        }
    });

    function getDragAfterElement(container, y) {
        const draggableElements = [...container.querySelectorAll('.draggable:not(.dragging)')];

        return draggableElements.reduce((closest, child) => {
            const box = child.getBoundingClientRect();
            const offset = y - box.top - box.height / 2;

            if (offset < 0 && offset > closest.offset) {
                return { offset: offset, element: child };
            } else {
                return closest;
            }
        }, { offset: Number.NEGATIVE_INFINITY }).element;
    }
}

