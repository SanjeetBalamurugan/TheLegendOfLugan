# 🎨 Stylized Hand-Painted PBR Shader (URP Shader Graph)

This guide walks you through creating a **hand-painted PBR shader** in Unity URP using Shader Graph  
(similar to WoW, Sea of Thieves, Genshin Impact).

---

## 1) Create the Shader Graph
1. **Assets → Create → Shader Graph → URP → Lit Shader Graph**  
2. Name it `SG_StylizedHandPainted_Lit`.  
3. Open it, then in **Graph Inspector**:  
   - **Surface:** Opaque (or Transparent if needed)  
   - **Workflow:** Metallic  
   - **Two-Sided:** Off (enable if needed)  
   - **Specular Highlights:** On  
   - **Environment Reflections:** On  

---

## 2) Blackboard Properties
Add these **Properties**:

### Textures
- `BaseColorMap (Texture2D)`  
- `NormalMap (Texture2D)` *(Texture Type: Normal in import)*  
- `RoughnessMap (Texture2D)` *(sRGB OFF in import)*  
- `AOMap (Texture2D)` *(sRGB OFF in import)*  
- `HeightMap (Texture2D)` *(optional, sRGB OFF)*  

### Colors & Sliders
- `BaseColorTint (Color)` → white  
- `Metallic (Float 0–1)` → **0.0**  
- `Smoothness (Float 0–1)` → **0.55**  
- `RoughnessInfluence (Float 0–1)` → **0.7**  
- `NormalStrength (Float 0–2)` → **0.6**  
- `AO_AlbedoStrength (Float 0–1)` → **0.3**  
- `AO_PBRStrength (Float 0–1)` → **1.0**  
- `Saturation (Float 0–2)` → **1.1**  
- `Contrast (Float 0.7–1.5)` → **1.08**  
- `ColorPower (Float 0.8–1.2)` → **1.0**  
- `RimColor (Color)` → teal or warm tone  
- `RimPower (Float 1–8)` → **3.0**  
- `RimIntensity (Float 0–2)` → **0.5**  

---

## 3) Node Wiring

### A) Base Color + AO + Stylization
1. **Sample BaseColorMap** → `BaseCol`  
2. Multiply with `BaseColorTint` → `TintedCol`  
3. **Sample AOMap** (use R channel) → `AO`  
4. Multiply `TintedCol * AO` → `AOColored`  
5. Lerp(`TintedCol`, `AOColored`, `AO_AlbedoStrength`) → `PaintedCol`  
6. Add **Saturation Control** (Gray → Lerp → `Saturation`) → `SatCol`  
7. Add **Contrast Control** (Subtract → Multiply → Add) → `ConCol`  
8. Add **Power/Gamma Control** (Power node with `ColorPower`) → `FinalAlbedo`  
9. Connect → **Base Color**  

### B) Normal Map
1. Sample `NormalMap` (Tangent Space) → `NrmTS`  
2. Apply `Normal Strength` with `NormalStrength` → `NrmTS_Str`  
3. Connect → **Normal**  

### C) Roughness → Smoothness
1. Sample `RoughnessMap` (R channel) → `Rgh`  
2. One Minus → `SmFromMap`  
3. Lerp(`Smoothness`, `SmFromMap`, `RoughnessInfluence`) → `SmOut`  
4. Connect → **Smoothness**  
5. Metallic = `Metallic`  

### D) Occlusion
1. Multiply `AO * AO_PBRStrength` → `AO_PBR`  
2. Connect → **Occlusion**  

### E) Rim Light
1. Fresnel → Power(`RimPower`)  
2. Multiply with `RimColor` → `RimCol`  
3. Multiply with `RimIntensity` → `RimEmit`  
4. Connect → **Emission**  

### F) (Optional) Height / Parallax
- Use `HeightMap` with **Parallax Occlusion Mapping** (if available).  
- Drive UVs for BaseColor & Roughness.  
- Keep amplitude very small (0.005–0.02).  

---

## 4) Hook Outputs to Master Node
- **Base Color** → `FinalAlbedo`  
- **Normal** → `NrmTS_Str`  
- **Metallic** → `Metallic`  
- **Smoothness** → `SmOut`  
- **Occlusion** → `AO_PBR`  
- **Emission** → `RimEmit`  

---

## 5) Texture Import Settings
- **BaseColorMap** → sRGB ON  
- **NormalMap** → Texture Type: Normal Map  
- **RoughnessMap** → sRGB OFF  
- **AOMap** → sRGB OFF  
- **HeightMap** → sRGB OFF  

---

## 6) Good Starting Values
- NormalStrength → **0.6**  
- Smoothness → **0.55**  
- RoughnessInfluence → **0.7**  
- Metallic → **0.0** (non-metals)  
- AO_AlbedoStrength → **0.3**  
- AO_PBRStrength → **1.0**  
- Saturation → **1.1**  
- Contrast → **1.08**  
- ColorPower → **1.0**  
- RimPower → **3.0**  
- RimIntensity → **0.5**  

---

## 7) Lighting Tips
- Strong **Directional Light** for key lighting  
- Add **Reflection Probes**  
- **URP Volume**:  
  - Bloom (low, soft)  
  - Color Adjustments: +10–20% saturation, +10% contrast  
  - Vignette (optional for focus)  

---

## 8) Troubleshooting
- **Flat look?** → Increase `AO_AlbedoStrength`, add reflection probes  
- **Too shiny?** → Lower `Smoothness` or reduce `RoughnessInfluence`  
- **Normals too harsh?** → Reduce `NormalStrength`  
- **Washed out?** → Check **Project Settings → Player → Color Space = Linear**  
