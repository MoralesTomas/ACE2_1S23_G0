const currentCache = "static"

self.addEventListener("install", e => {
    e.waitUntil(
        caches.open(currentCache).then(cache => {
            return cache.addAll(["./", "./src/styles.css", "./images/logo192.png", "./images/logo512.png", "./src/scripts.js", "./src/graph.js"]);
        })
    );
});

self.addEventListener('activate', e => {
    e.waitUntil(
        caches.keys().then(cacheNames => Promise.all(
            cacheNames.filter(cacheName => {
                return cacheName !== currentCache
            }).map(cacheName => caches.delete(cacheName))
        ))
    )
});

/* self.addEventListener("fetch", e => {
    e.respondWith(
        fetch(e.request).catch(() => {
            console.log(e.request);
            return caches.match(e.request);
        })
    );
}); */

// fetch the resource from the network
const fromNetwork = (request, timeout) =>
  new Promise((fulfill, reject) => {
    const timeoutId = setTimeout(reject, timeout);
    fetch(request).then(response => {
      clearTimeout(timeoutId);
      fulfill(response);
      update(request);
    }, reject);
  });

// fetch the resource from the browser cache
const fromCache = request =>
  caches
    .open(currentCache)
    .then(cache =>
      cache
        .match(request)
        .then(matching => matching || cache.match('/offline/'))
    );

// cache the current page to make it available for offline
const update = request => 
  caches
    .open(currentCache)
    .then(cache =>
      fetch(request).then(response => cache.put(request, response))
    );


self.addEventListener('fetch', event => {
    // console.log(e.request)
    const request = new Request(event.request, {
      method: event.request.method,
      headers: event.request.headers,
      mode: event.request.mode,
      credentials: event.request.credentials,
      cache: event.request.cache,
      redirect: event.request.redirect,
      referrer: event.request.referrer,
      integrity: event.request.integrity
    });

    event.respondWith(
        fromNetwork(request, 10000).catch(() => fromCache(request))
    );
    event.waitUntil(update(request));
});

// self.addEventListener('fetch', function(event) {
//   event.respondWith(
//     fetch(event.request)
//       .then(function(response) {
//         // Si la petición se completa correctamente, se guarda en la caché para futuras solicitudes.
//         caches.open('nombre_de_la_cache')
//           .then(function(cache) {
//             cache.put(event.request, response.clone());
//           });
//         return response;
//       })
//       .catch(function() {
//         // Si la petición falla, se busca en la caché.
//         return caches.match(event.request);
//       })
//   );
// });
