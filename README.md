🧩 Tangram Puzzle – Unity Developer Test

A simple, kid-friendly tangram puzzle game built in Unity 6 (6000.0.38f1) as part of a Unity Developer technical test.

The goal is to drag, rotate, and snap tangram pieces into their correct template positions.

🎮 Gameplay Overview

Drag pieces using mouse (or touch)

Rotate pieces to match the template

Pieces snap and lock when placed correctly

Audio feedback for interactions

Puzzle completes when all pieces are snapped

Designed for ages 4–10 with forgiving snap tolerance and clear feedback.

✨ Features Implemented

Smooth top-down drag on XZ plane

Rotation via mouse / keyboard (step-based)

Collision + rotation-based snapping

Locking pieces after correct placement

Centralized audio feedback:

Click → interaction / rotation

Pop → correct snap

Bump → incorrect placement

Puzzle completion detection using event-based architecture

Clean separation between Piece, Target, and GameManager logic

🧠 Technical Notes / Design Decisions

Top-down camera with fixed Y-plane movement

Used ray-plane intersection for accurate dragging

Avoided FindObjectOfType in gameplay flow

Used events (OnPieceSnapped) to decouple Piece and GameManager

Snap logic checks rotation tolerance instead of exact values to keep gameplay forgiving for kids

Audio handled via a centralized AudioManager

🛠 Controls (Desktop)
Action	Input
Drag piece	Left Mouse Button
Rotate (free)	Right / Middle Mouse Button
Rotate (step)	R / E
Reset piece	Space

Note: Rotation input can easily be swapped with UI buttons for mobile.

📁 Project Structure (Key Scripts)
Assets/
 ├── Scripts/
 │   ├── Piece.cs        // Drag, rotate, snap logic
 │   ├── Target.cs       // Snap targets
 │   ├── GameManager.cs  // Puzzle completion
 │   └── AudioManager.cs // Sound effects

⏱ Estimated Time Spent

~2.5 hours

Focused on:

Core gameplay feel

Clean code & architecture

Kid-friendly interaction

Light polish (audio feedback)

🚀 Possible Improvements (With More Time)

Mobile-friendly rotate UI button

Visual snap preview / glow

Confetti + win animation

Multiple puzzle levels

Accessibility improvements

🔧 Unity Version

Unity 6000.0.38f1 (Unity 6 LTS)

📩 Submission Notes

This project prioritizes clarity, gameplay feel, and maintainable code over feature completeness, as recommended in the test instructions.
