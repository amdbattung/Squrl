window.infiniteSelect = {
    initialize: function (container, dotNetReference) {
        if (!container) {
            return;
        }

        if (container.dataset.initialized === "true") {
            return;
        }

        container.dataset.initialized = "true";

        container._outsidePointerHandler = function (event) {
            if (!container.contains(event.target)) {
                dotNetReference.invokeMethodAsync(
                    "CloseDropdown"
                );
            }
        };

        document.addEventListener(
            "pointerdown",
            container._outsidePointerHandler
        );
    },

    initializeInfiniteScroll: function (sentinel, dotNetReference) {
        if (!sentinel) {
            return;
        }

        if (sentinel._observer) {
            sentinel._observer.disconnect();
        }

        sentinel._observer = new IntersectionObserver(
            function (entries) {
                if (!entries[0].isIntersecting) {
                    return;
                }

                dotNetReference.invokeMethodAsync(
                    "LoadNextPage"
                );
            },
            {
                root: sentinel.parentElement,
                rootMargin: "100px",
                threshold: 0
            }
        );

        sentinel._observer.observe(sentinel);
    },

    dispose: function (container) {
        if (!container) {
            return;
        }

        if (container._outsidePointerHandler) {
            document.removeEventListener(
                "pointerdown",
                container._outsidePointerHandler
            );

            container._outsidePointerHandler = null;
        }
    }
};