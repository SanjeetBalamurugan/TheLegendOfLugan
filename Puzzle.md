# 🎮 Game Design Prompt — *Chain Reaction*

## Elevator Pitch
A narrative-driven puzzle adventure where **every action triggers the next**. Players solve **cause-and-effect chain reactions** across Mayan-inspired ruins using:
- **The Ancient Device** (gifted in Level 1) to **record, queue, link, and rewind** triggers.
- **An Elemental Bow** (Fire, Water, Earth, Air) gifted by a **mysterious girl** who actively guides the MC with hints, gestures, and short tutorials.

---

## Narrative Setup
- The **MC** enters overgrown temple ruins seeking a lost artifact tied to time and cycles.
- A **mysterious girl**—equal parts mentor and enigma—appears in Level 1. She:
  - Gifts the **Ancient Device** and **her Elemental Bow**.
  - Teaches puzzle “grammar” through contextual guidance (subtle highlights, whispered tips).
  - Occasionally demonstrates a shorter chain, prompting the player to complete the full sequence.

---

## Theme & Pillars
1. **Chain Reaction:** Rube-Goldberg style sequences where the **order** of actions matters.
2. **Elemental Interplay:** Fire, Water, Earth, Air interactions with the environment.
3. **Sequential Logic:** Plan → Prime → Trigger → Cascade → Verify (with Device).
4. **Environmental Agency:** Mirrors, cranks, gears, ropes, weights, pendulums, water channels.
5. **Readable Systems:** Clear feedback (sounds, glow, motion) for each link in the chain.

---

## Setting & Progression
- **Levels 1–5 (Outside: Teaching Phase):** Courtyards, ritual gardens, weathered shrines. Focus on single-element skills and short chains. The girl mentors frequently.
- **Levels 6–10 (Inside: Mastery Phase):** Mural halls, mirror chambers, gearworks, elemental sanctums. Longer multi-element chains, precise timing, and Device sequencing mastery. The girl’s hints become more cryptic and optional.

---

## Core Tools

### Ancient Device (from Level 1)
- **Link Mode:** Connect eligible mechanisms (A → B → C) to visualize a potential chain.
- **Queue Mode:** Stage up to *N* actions; execute on trigger.
- **Rewind Snap:** Rewind the **last link** without resetting the entire puzzle.
- **Ghost Trace:** Shows the **last successful chain** as a faint timeline for learning.
- **Safety Gate:** Prevents contradictory states; UI shows conflicts before execution.

### Elemental Bow (from Level 1)
- **Fire:** Light torches, burn ropes, ignite braziers/crystals to emit light beams.
- **Water:** Fill channels, grow vines, power waterwheels, cool overheated devices.
- **Earth:** Raise platforms with stone growth, collapse cracked walls, add weight.
- **Air:** Spin fans/windmills, push pendulums, rotate dials, carry embers/scent.

---

## Puzzle Grammar (Design Rules)
- **Binary Links:** “If A happens, then B unlocks” (e.g., rope burns → weight drops).
- **Analog Links:** Gradients matter (e.g., water level = platform height).
- **Temporal Links:** Windows and phases (e.g., door open for 6s; pendulum peak).
- **Stateful Links:** Persistent toggles (mirror orientation, gear alignment).
- **Counterpoint:** One element aids another (Air moves Fire’s ember; Water grows vine for Earth weight).

---

## Difficulty Curve
- **Levels 1–2:** One-element chains, 2–3 links; Device basics (Link, Rewind Snap).
- **Levels 3–5:** Two-element combos, timing windows, 4–6 links; introduce Queue Mode.
- **Levels 6–8:** Multi-branch chains, resource balancing, mirrored states, 6–9 links.
- **Levels 9–10:** Full four-element orchestration, cross-room dependencies, 10–14 links, precise Device sequencing.

---

## Example Interactions (Use Liberally)
- **Mirrors/Prisms:** Redirect fire-light to mural receptors → gear arrays engage.
- **Cranks/Gears:** Set pre-rotation; chain provides final torque.
- **Ropes/Weights:** Burn to drop, or wet to delay burn rate.
- **Waterworks:** Partial fills create height “keys”; drains reset analog states.
- **Pendulums:** Air establishes rhythm; collision breaks cracked supports at apogee.
- **Boulders/Blocks:** Earth adds mass; wind nudges trajectories; water floats/logistics.

---

## Guidance & UX (The Girl as Diegetic Tutor)
- **First Exposure:** She demonstrates one safe link (A → B), then invites the player to extend (B → C → D).
- **Hint Cadence:** Soft cues first (glance, gesture, footstep path). Hold-for-hint reveals a verbal nudge. Optional “Show Chain Skeleton” after failure streaks.
- **Diegetic UI:** Device rings light up in the order of planned links; mislinks glow red; time windows pulse.

---

## Failure & Recovery
- **Local Rewind:** Roll back the last link without wiping setup.
- **Soft Reset:** Drain channels, respawn boulders, re-grow ropes over time if watered.
- **Learning Echo:** After failure, show a **ghost trace** of the attempted chain.

---

## Success Criteria
- Players can **predict** outcomes before firing.
- Chains are **watchable and delightful**, rewarding foresight.
- Elemental affordances are **consistent** and teachable.
- The girl’s guidance feels **supportive, not hand-holdy**.
- The Device empowers **experimentation** over punishment.

---

## Content Targets (High Level)
- **Outside (1–5):** 
  - 1) Fire + ropes; Device Link/Rewind introduced by the girl.
  - 2) Water growth + weight balancers.
  - 3) Air timing + windmills → rolling boulder.
  - 4) Earth mass + dual plates → bridge halves.
  - 5) First multi-element trio + queued execution.

- **Inside (6–10):**
  - 6) Mirror light networks + simultaneous receptors.
  - 7) Water level puzzles + floaters + controlled drains.
  - 8) Air-driven pendulums + sand weights + breakpoints.
  - 9) Four-element orchestration with Device sequencing gates.
  - 10) Grand chain: cross-room triggers, mirrored crystals, waterwheels, statue locks.

---

## Tone & Aesthetics
- **Mayan-inspired** motifs and geometry with respectful abstraction.
- Overgrown **bioluminescent flora**, warm torchlight vs. cool temple stone.
- Soundscape tracks chain progress: click → whirr → thunk → chime.

---

## Accessibility
- Toggle **action speed** and **timing window leniency**.
- **Color-independent** glyphs for element feedback.
- Optional **auto-aim assist** & **aim time-slow** when drawing the bow.

---

## Production Notes
- Build puzzles around **clear cause visibility** (player can see 80% of the chain from a vantage).
- Favor **short backtracks** and **layered resets** over full hard resets.
- Authoring tools: node-graph for links, tags for elemental affordances, timeline scrubber for Device playback.
