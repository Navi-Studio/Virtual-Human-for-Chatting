[English](Description.md) / [日本語](Description.ja.md)

---

# Multiple Models

To allow easy interaction with other 2D elements, Cubism model parts are by default sorted using Z-offsetting. This makes it necessary to sort multiple models either by Unity's sorting layers and/or sorting orders. Both can easily be achieved via the ``CubismRenderController.SortingLayer`` and ``CubismRenderController.SortingOrder`` settings.

By clearly separating the sorting of models, you not only get the expected visible results, but also make it possible for Unity's dynamic batching to work (of course, only if enabled).
