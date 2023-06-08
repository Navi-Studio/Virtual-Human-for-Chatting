[English](Description.md) / [日本語](Description.ja.md)

---

# Masking

Masking is automatically set up for Cubism models that use masks on import (and whenever you manually call ``CubismModel3Json.ToModel()``).

By default, however, all models share the same global GPU buffer for intermediate mask data (``CubismMaskTexture.GlobalMaskTexture``). If your models use more masks than fit into that buffer, be sure to either adjust the buffer, or assign custom buffers to your models via ``CubismMaskController.MaskTexture``.

The quad on the scene shows the layout of the masks in the GPU buffer.
