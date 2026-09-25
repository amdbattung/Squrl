window.infiniteSelect = {
    initialize: function (container, dotNetReference) {

        if (!container) {
            return;
        }

        if (container.dataset.initialized === "true") {
            return;
        }

        container.dataset.initialized = "true";

        container._outsidePointerHandler =
            function (event) {

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

    observe: function (
        container,
        sentinel,
        dotNetReference
    ) {

        if (!container || !sentinel) {
            return;
        }

        if (container._infiniteObserver) {

            container._infiniteObserver.disconnect();

            container._infiniteObserver = null;
        }

        container._infiniteObserver =
            new IntersectionObserver(
                function (entries) {

                    const entry =
                        entries[0];


                    if (!entry ||
                        !entry.isIntersecting) {

                        return;
                    }

                    /*
                     * Stop observing while the request
                     * is being processed.
                     */
                    if (container._infiniteObserver) {

                        container._infiniteObserver.disconnect();

                        container._infiniteObserver =
                            null;
                    }

                    dotNetReference.invokeMethodAsync("LoadNextPage")
                        .catch(function (error) {
                            
                        });

                },
                {
                    /*
                     * Use the browser viewport as the
                     * intersection root.
                     */
                    root: null,

                    /*
                     * Begin loading before the user
                     * reaches the sentinel.
                     */
                    rootMargin: "200px",

                    threshold: 0
                }
            );

        container._infiniteObserver.observe(
            sentinel
        );
    },

    dispose: function (container) {

        if (!container) {
            return;
        }

        if (container._infiniteObserver) {
            container._infiniteObserver.disconnect();
            container._infiniteObserver = null;
        }


        if (container._outsidePointerHandler) {

            document.removeEventListener(
                "pointerdown",
                container._outsidePointerHandler
            );

            container._outsidePointerHandler = null;
        }
        
        container.dataset.initialized =
            "false";
    }
};