// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Register TaskFlow Service Worker
if ("serviceWorker" in navigator) {
    window.addEventListener("load", () => {
        navigator.serviceWorker.register("/service-worker.js")
            .then(registration => {
                console.log("TaskFlow Service Worker registered:", registration.scope);
            })
            .catch(error => {
                console.error("TaskFlow Service Worker registration failed:", error);
            });
    });
}
