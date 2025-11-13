# **GAME DESIGN DOCUMENT – *Working Title: Linebreak***

**Platform:** PC (C# standalone, no engine)
**Rendering:** ASCII/ANSI terminal graphics + optional minimal custom draw libraries
**Distribution:** Itch.io (packaged via .NET self-contained build)
**Genre:** Narrative Hacking Simulation / Investigative Suspense
**Themes:** Government trust, corporate power, economic inequality, epistemic humility, conspiratorial thinking

---

## **1. High-Level Concept**

*Linebreak* is a terminal-based hacking and diagnostics simulation. You begin as a contracted support technician maintaining a state-civilian data exchange node. Early tasks are harmless: decoding logs, patching file systems, fixing misrouted packets. As the player learns the tools, they stumble on irregularities hinting at multiple possible conspiracies.

Some anomalies are genuine systemic failures or illicit operations. Others are coincidental noise that look suspicious but are ultimately meaningless. The game’s moral centre lies in whether the player cultivates discernment… or spirals into their own invented mythology.

Your beliefs shape your trajectory. What you *think* is happening becomes the reality you act upon, which meaningfully alters political outcomes, public trust, and individual lives.

---

## **2. Core Pillars**

### **2.1 Learning by Doing**

Early missions teach actual gameplay functions disguised as workplace tasks. Skills include:

* Packet trace inspection
* Log parsing
* Message queue repair
* Process monitoring
* Decrypting simple ciphers
* Patching configuration files
* Containment of malware simulations
* Synthetic “field reports” cross-matching

### **2.2 Ambiguous Information Space**

The world is noisy. The system generates:

* **Truthful anomalies** (real conspiracies)
* **False patterns** (coincidences that resemble skulduggery)
* **Artificial distractions** (corporate PR dumps, government spin)
* **Data artefacts** (corrupted entries, redacted fields, truncated logs)

Player interpretation becomes a mechanic.

### **2.3 Consequence Through Action**

Belief drives choices:

* Report up the chain
* Leak to civilians
* Contact underground activists
* Delete evidence
* Fabricate narratives
* “Fix” systems in biased ways
* Remain neutral and procedural

The simulation tracks public trust, government stability, corporate influence, and class tension across sectors.

---

## **3. Narrative Overview**

### **3.1 Act I: Routine Maintenance**

* You’re hired to maintain a small infrastructure node.
* Interface and tools are introduced gradually.
* First oddity: a series of mismatched timestamps that hint at unauthorized remote injections.
* This may be an innocent desynchronization… or something more.

### **3.2 Act II: Diverging Threads**

The player can pursue multiple leads:

* **Government overreach conspiracy** (some true, some false).
* **Corporate sabotage** targeting competing contractors.
* **Grassroots hacktivist movement** with fragmented motives.
* **Economic manipulation scandal** involving data redirection to influence public policy.
* **Two full red herrings** that produce no meaningful end if chased.

Player actions open or close branches.

### **3.3 Act III: Escalation**

The node’s data begins influencing national sentiment. The player sees ripple effects:

* Public forums change tone
* Corporate memos shift strategy
* Ministers request new data policies
* Civil-unrest sentiment spikes

The player now decides:

* Reveal?
* Suppress?
* Shape?

### **3.4 Act IV: Resolution Paths**

The story resolves according to:

* Which leads the player trusted
* Which leads they ignored
* Whether they acted out of duty, curiosity, paranoia, or self-interest
* How precisely they executed technical tasks

Endings vary across:

* Government stability
* Corporate collapse or dominance
* Public distrust or renewal
* Player liberation, employment termination, imprisonment, or becoming a folk legend
* Complete epistemic failure (you chased ghosts and broke everything)

### **3.5 Act V: Post-Mortem Reflection**

A meta-summary reveals the objective truth behind all conspiracies, letting the player compare their beliefs to reality. This is optional and can be disabled for replay value.

---

## **4. Gameplay Systems**

### **4.1 Terminal Interaction**

* Built purely in C# console mode or an ANSI graphics wrapper
* Windows/Linux support
* Commands: `trace`, `decode`, `map`, `patch`, `process`, `logview`, `msgqueue`, `connect`, `flag`, `leak`, `autoroute`, `witness`, etc.
* Command chaining: `trace | decode -cipher=rot13 | flag --suspicious`

### **4.2 Evidence Mapping**

Internally represented as:

* A graph of nodes (data points)
* Each node flagged as true/false/noise/ambiguous
* Player actions reveal or hide edges
* Conspiracy webs can be misinterpreted if the wrong links are assumed

### **4.3 Reputation & Influence System**

Four major meters (not visible at start):

* Government Trust
* Corporate Influence
* Public Anxiety
* Player Credibility

Every choice adjusts these.

---

## **5. Tools & UI**

### **5.1 Visual Style**

* Retro terminal palette
* ASCII glyph animations
* Minimal bar graphs and line charts
* Occasional glitch effects tied to narrative beats

### **5.2 Optional “GraphView”**

A 2D ASCII graph renderer using:

```
│── Node_A  
└── Node_B  
    └── Node_C…
```

### **5.3 Logfile Viewer**

Highlighting suspicious segments via syntax-like color coding.

---

## **6. Player Progression**

* Unlock new modules as you complete repairs
* Each module increases both power and risk
* Narrative paths adjust based on tool misuse or overreach

---

## **7. Conspiracies (Examples)**

### **7.1 True Ones**

* A real inter-agency data laundering loop
* Corporate black-bag team hiding downtime spikes
* Manipulated eligibility statistics affecting public services

### **7.2 False Ones**

* Coincidental overlapping errors
* Timestamp drift
* Log rot that looks like tampering
* Dummy messages seeded by a stress-test tool

Player interpretation is the whole point.

---

## **8. Endings**

At least 12 major arcs:

1. Loyal technician stabilizes the system
2. Whistleblower sparks reform
3. Paranoid meltdown causing national confusion
4. Corporate kingmaker
5. Unwitting tool of extremists
6. Public hero
7. Blacklisted and hunted
8. Digital recluse
9. Denial of conspiracy (missing real corruption)
10. Unraveled truth (balanced discernment)
11. Manufactured conspiracy (player invents events)
12. Perfect storm: player collapses all trust metrics

---

## **9. Technical Architecture (C#)**

### **9.1 Recommended Stack**

* .NET 8 single-file publish
* Terminal UI library: **Spectre.Console** or **gui.cs** (Terminal.Gui)
* SQLite embedded for save data
* Pure data simulation in service classes
* JSON for narrative and events

### **9.2 Module Layout**

```
/src
  /Core
  /UI
  /Simulation
  /Conspiracies
  /Commands
  /Saves
  /Data
```

### **9.3 Packaging**

`dotnet publish -c Release -r win-x64 --self-contained true --output dist/`

---

## **10. Development Roadmap**

1. **Prototype** terminal commands + basic repairs
2. Build noise generation + anomaly engine
3. Integrate reputation/trust metrics
4. Add conspiracies + red herrings
5. Add narrative events + branching logic
6. Add ending resolver
7. Playtest for epistemic difficulty
8. Package for Itch.io