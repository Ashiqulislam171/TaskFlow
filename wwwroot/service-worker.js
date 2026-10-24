const CACHE_NAME = "taskflow-static-v1";

const STATIC_ASSETS = [
    "/css/site.css",
    "/js/site.js"
];

self.addEventListener("install", event => {
    event.waitUntil(
        caches.open(CACHE_NAME).then(cache => cache.addAll(STATIC_ASSETS))
    );

    self.skipWaiting();
});

self.addEventListener("activate", event => {
    event.waitUntil(
        caches.keys().then(keys =>
            Promise.all(
                keys
                    .filter(key => key !== CACHE_NAME)
                    .map(key => caches.delete(key))
            )
        )
    );

    self.clients.claim();
});

self.addEventListener("fetch", event => {
    const request = event.request;
    const url = new URL(request.url);

    if (
        request.method !== "GET" ||
        url.origin !== self.location.origin ||
        url.pathname.startsWith("/Account") ||
        url.pathname.startsWith("/Tasks") ||
        url.pathname.startsWith("/Identity")
    ) {
        return;
    }

    if (request.destination === "document") {
        event.respondWith(
            fetch(request).catch(() =>
                new Response(
                    "You are offline. Please reconnect to the internet and try again.",
                    {
                        status: 503,
                        headers: { "Content-Type": "text/plain; charset=utf-8" }
                    }
                )
            )
        );

        return;
    }

    if (STATIC_ASSETS.includes(url.pathname)) {
        event.respondWith(
            caches.match(request).then(cached =>
                cached || fetch(request)
            )
        );
    }
});
