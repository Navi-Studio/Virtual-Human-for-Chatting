[English](Description.md) / [日本語](Description.ja.md)

---

# LookAt

This sample demonstrates the look functionality. Looking must be set up manually, but setting it up isn't too cumbersome. You simply have to add a ``CubismLookController`` component to the model game object, and ``CubismLookParameter``s to any parameter you want to have participate in the look motion.

Your model will follow any ``ICubismLookTarget`` you assign to ``CubismLookContoller.Target``.
