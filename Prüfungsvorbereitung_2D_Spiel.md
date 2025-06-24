# Prüfungsvorbereitung: 2D-Spiel mit OpenTK und C# 

## 1. Game Loop - Das Herz des Spiels

### Implementierung in unserem Projekt

**Hauptklasse**: `Core/game.cs:75-434`

```csharp
public abstract class Game {
    public abstract void StartGame();
    protected abstract void Init();
    protected abstract void Update(float deltaTime);
    protected abstract void Render(float deltaTime);
    
    // Der Hauptloop läuft in diesen Events:
    this.window.UpdateFrame += (FrameEventArgs eventArgs) => {
        this.Update_Game_Time((float)eventArgs.Time);
        this.playerController.Update_Internal(Game_Time.delta, this.inputEvent);
        this.activeMap.update_internal(Game_Time.delta);
    };
    
    this.window.RenderFrame += (FrameEventArgs eventArgs) => {
        this.window.SwapBuffers();
        this.Internal_Render();
        this.Imgui_Render(Game_Time.delta);
    };
}
```

### Game Loop Struktur:
1. **Input Processing**: Keyboard/Mouse Events werden in `inputEvent` Liste gesammelt
2. **Update Logic**: `Update_Game_Time()` sorgt für Frame Rate Independence
3. **Rendering**: `Internal_Render()` zeichnet alles mit Double Buffering

### Frame Rate Independence
**Code**: `Core/game.cs:199-202`
```csharp
private void Update_Game_Time(float deltaTime) {
    Game_Time.delta = deltaTime;  // Zeit seit letztem Frame
    Game_Time.total += deltaTime; // Gesamtspielzeit
}
```

**Verwendung**: `Core/world/character.cs:100`
```csharp
Vector2 velocity = direction * speed * Game_Time.delta;
```

## 2. Kollisionssystem mit Box2D

### Box2D Integration

**Hauptklasse**: `Core/physics/collider.cs:9-117`

```csharp
public sealed class Collider {
    public Collision_Shape shape;     // Circle, Square, None
    public Collision_Type type;       // world, character, bullet
    public Body body { get; set; }    // Box2D Physics Body
    public float mass;
    public Vector2 velocity { get; set; }
}

public enum Collision_Shape {
    None = 0, Circle = 1, Square = 2,
}

public enum Collision_Type {
    None = 0, world = 1, character = 2, bullet = 3,
}
```

### Wie Box2D unter der Haube funktioniert:

1. **Broad Phase**: Box2D verwendet eine hierarchische Bounding Volume Hierarchy (Dynamic AABB Tree) um schnell Objektpaare zu finden, die potenziell kollidieren könnten.

2. **Narrow Phase**: Für jedes Objektpaar wird ein genauer Kollisionstest durchgeführt:
   - **Circle vs Circle**: Distanzvergleich zwischen Mittelpunkten
   - **AABB vs AABB**: Separating Axis Theorem auf X- und Y-Achse
   - **Circle vs AABB**: Closest Point Algorithmus

3. **Collision Response**: Box2D berechnet Kontaktpunkte, Normale und wendet Impulse an

### Physics World Setup
**Code**: `Core/world/map.cs:36-47`

```csharp
public Map() {
    AABB aabb = new();
    aabb.LowerBound.Set(-100000, -100000);
    aabb.UpperBound.Set(100000, 100000);
    
    Vec2 gravity = new(0.0f, 0.001f);  // Minimale Schwerkraft
    physicsWorld = new World(aabb, gravity, true);
    physicsWorld.SetContactListener(new CollisionListener());
}
```

### Character Kollision
**Code**: `Core/world/map.cs:156-199`

```csharp
public Character Add_Character(Character character, Vector2? position = null) {
    BodyDef def = new();
    def.LinearDamping = 1.0f;  // Reibung
    def.AllowSleep = false;    // Nie einschlafen
    
    CircleDef circleDef = new();
    circleDef.Radius = Math.Abs(character.transform.size.X / 2);
    circleDef.Density = 1f;
    circleDef.Friction = 0.3f;
    
    Body body = physicsWorld.CreateBody(def);
    body.CreateShape(circleDef);
    body.IsDynamic();
    body.SetMassFromShapes();
    body.SetUserData(character);  // Verknüpfung für Callbacks
}
```

### Raycast für KI-Sicht
**Code**: `Core/world/map.cs:51-78`

```csharp
public bool ray_cast(Vector2 start, Vector2 end, out Vec2 normal, 
                     out float distance, out Game_Object? intersected_game_object) {
    Segment ray = new() {
        P1 = new Vec2(start.X, start.Y),
        P2 = new Vec2(end.X, end.Y),
    };
    
    var shape = physicsWorld.RaycastOne(ray, out distance, out normal, false, null);
    intersected_game_object = shape != null ? (Game_Object)shape.GetBody().GetUserData() : null;
    
    return shape != null;
}
```

## 3. Transformations & Koordinatensystem

### Transform System
**Hauptklasse**: `Core/util/transform.cs:7-169`

```csharp
public class Transform {
    public Vector2 size { get; set; } = new Vector2(0);
    public float rotation { get; set; } = 0;
    public Transform? parent { get; set; }
    
    private Vector2 positionValue;
    
    // Hierarchisches Koordinatensystem
    public Vector2 position {
        get {
            if (this.parent == null)
                return this.positionValue;
            else
                return this.parent.position + this.positionValue;  // Weltposition
        }
        set { 
            this.positionValue = this.parent == null ? value : value - this.parent.position; 
        }
    }
}
```

### Matrix-Transformationen
**Code**: `Core/util/transform.cs:72-87`

```csharp
public Matrix4 GetTransformationMatrix() {
    Vector2 position = this.position;
    Matrix4 translation = Matrix4.CreateTranslation(new Vector3(position.X, position.Y, 0));
    Matrix4 scale = Matrix4.CreateScale(new Vector3(this.size.X, this.size.Y, 1));
    Matrix4 rotation = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(this.rotation));
    
    return scale * rotation * translation;  // SRT Order
}
```

### Camera Shake System
**Code**: `Core/util/transform.cs:104-150`

```csharp
public void ApplyShake(ShakeProfile profile) {
    if (currentShakeProfile == null || shakeIntensity < profile.Intensity) {
        currentShakeProfile = profile;
        shakeIntensity = profile.Intensity;
        shakeDecay = profile.Decay;
        if (!isShaking) {
            originalPosition = this.position;
            isShaking = true;
        }
    }
}

public void Update() {
    if (isShaking) {
        Vector2 shakeAmount = new Vector2(
            (float)(random.NextDouble() * 2 - 1) * shakeIntensity * shakeMagnitude,
            (float)(random.NextDouble() * 2 - 1) * shakeIntensity * shakeMagnitude);
        position += shakeAmount;
        
        shakeIntensity *= shakeDecay;  // Exponentieller Abfall
        position = Vector2.Lerp(position, originalPosition, Math.Clamp(1 - shakeIntensity, 0, 1));
    }
}
```

### Kamera System
**Hauptklasse**: `Core/world/camera.cs:6-158`

```csharp
public sealed class Camera : Game_Object {
    public Matrix4 Get_Projection_Matrix() {
        float left = this.transform.position.X - (this.transform.size.X / 2f);
        float right = this.transform.position.X + (this.transform.size.X / 2f);
        float top = this.transform.position.Y - (this.transform.size.Y / 2f);
        float bottom = this.transform.position.Y + (this.transform.size.Y / 2f);
        
        Matrix4 orthographic_matrix = Matrix4.CreateOrthographicOffCenter(left, right, bottom, top, .00f, 1000f);
        Matrix4 zoom_matrix = Matrix4.CreateScale(this.scale, this.scale, 1);
        
        return orthographic_matrix * zoom_matrix;
    }
}
```

## 4. Rendering System

### Moderne OpenGL Pipeline

**Vertex Shader**: `Core/defaults/shaders/texture_vert.glsl:1-15`
```glsl
#version 330 core

layout (location = 0) in vec2 position;
layout (location = 1) in vec2 in_tex_coord;

uniform mat4 projection;  // Kamera-Matrix
uniform mat4 model;       // Object-zu-World-Matrix

out vec2 tex_coord;

void main() {
    gl_Position = projection * model * vec4(position, 0, 1);
    tex_coord = in_tex_coord;
}
```

**Fragment Shader**: `Core/defaults/shaders/texture_frag.glsl:1-12`
```glsl
#version 330 core

in vec2 tex_coord;
out vec4 fragColor;

uniform sampler2D u_texture[5];
uniform vec4 u_tint = vec4(1.0, 1.0, 1.0, 1.0);

void main() {
    vec4 texColor = texture(u_texture[0], tex_coord);
    fragColor = texColor * u_tint;  // Tint-System für Effekte
}
```

### Sprite System
**Hauptklasse**: `Core/world/sprite.cs:11-307`

```csharp
public sealed class Sprite : I_animatable {
    // VAO/VBO System für GPU-Daten
    private Index_Buffer indexBuffer;
    private Vertex_Buffer vertexBuffer;
    private Vertex_Array vertexArray;
    
    // UV-Koordinaten für Texture Atlas
    public Sprite Select_Texture_Region(int numberOfColumns = 1, int numberOfRows = 1, 
                                       int columnIndex = 0, int rowIndex = 0) {
        float offset_y = 1.0f / ((float)numberOfRows * 50);
        float offset_x = 1.0f / ((float)numberOfColumns * 50);
        
        // UV-Koordinaten für aktuellen Texture-Atlas-Bereich berechnen
        _verticies[3] = (numberOfRows - rowIndex - 1) / (float)numberOfRows + offset_y;
        _verticies[2] = columnIndex / (float)numberOfColumns + 1.0f / numberOfColumns - offset_x;
        
        vertexBuffer.Update_content(_verticies);
        return this;
    }
}
```

### Rendering Pipeline
**Code**: `Core/world/sprite.cs:193-235`

```csharp
public void Draw(Matrix4? model = null) {
    // 1. Texture binding
    texture?.Use(TextureUnit.Texture0);
    
    // 2. OpenGL State
    GL.Enable(EnableCap.Blend);
    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    
    // 3. Shader Setup
    shader.Use();
    vertexArray.Bind();
    indexBuffer.Bind();
    
    // 4. Matrix-Transformationen
    if (transform.mobility == Mobility.STATIC)
        shader.Set_Matrix_4x4("model", modelMatrix);  // Vorberechnet
    else
        shader.Set_Matrix_4x4("model", Calc_Modle_Matrix());  // Dynamisch
    
    // 5. Draw Call
    GL.DrawElements(PrimitiveType.Triangles, indeices.Length, DrawElementsType.UnsignedInt, 0);
}

private Matrix4 Calc_Modle_Matrix() {
    Matrix4 trans = Matrix4.CreateTranslation(transform.position.X, transform.position.Y, 0);
    Matrix4 sca = Matrix4.CreateScale(transform.size.X, transform.size.Y, 0);
    Matrix4 rot = Matrix4.CreateRotationZ(transform.rotation);
    return sca * rot * trans;  // Scale * Rotation * Translation
}
```

## 5. Partikelsystem

### Hochperformante Instanced Rendering
**Hauptklasse**: `Core/particles/ParticleSystem.cs:9-200`

```csharp
public class ParticleSystem {
    public const int MaxParticles = 100000;
    private float[] _particlePositionSizeData = new float[MaxParticles * 4];
    private byte[] _particleColorData = new byte[MaxParticles * 4];
    
    // Instanced Rendering Setup
    private void InitializeBuffers() {
        // Quad für alle Partikel (wird instanziiert)
        float[] quadVertices = {
            -0.5f, -0.5f, 0.0f,  // Bottom-left
             0.5f, -0.5f, 0.0f,  // Bottom-right
             0.5f,  0.5f, 0.0f,  // Top-right
            -0.5f,  0.5f, 0.0f   // Top-left
        };
        
        // Position/Size Buffer (per Instance)
        _instanceVBO = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _instanceVBO);
        GL.BufferData(BufferTarget.ArrayBuffer, MaxParticles * 4 * sizeof(float), 
                     IntPtr.Zero, BufferUsageHint.DynamicDraw);
        GL.VertexAttribDivisor(1, 1); // Update per instance
        
        // Color Buffer (per Instance)
        _colorVBO = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _colorVBO);
        GL.BufferData(BufferTarget.ArrayBuffer, MaxParticles * 4 * sizeof(byte), 
                     IntPtr.Zero, BufferUsageHint.DynamicDraw);
        GL.VertexAttribDivisor(2, 1); // Update per instance
    }
}
```

### Multi-Pass Rendering System
**Code**: `Core/particles/ParticleSystem.cs:163-197`

```csharp
public void Render() {
    if (_activeParticleCount == 0) return;
    
    _shader.Use();
    GL.BindVertexArray(_vao);
    
    // ===== PASS 1: DETAIL & CORE (Standard Blending) =====
    _shader.SetUniform("renderPass", 0);
    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    GL.DrawElementsInstanced(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 
                            IntPtr.Zero, _activeParticleCount);
    
    // ===== PASS 2: GLOW & SPARKLES (Additive Blending) =====
    _shader.SetUniform("renderPass", 1);
    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One); // Additive
    GL.DrawElementsInstanced(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 
                            IntPtr.Zero, _activeParticleCount);
}
```

### Physics-based Particle Update
**Code**: `Core/particles/ParticleSystem.cs:100-161`

```csharp
public void Update(float deltaTime, Vector2 playerPosition) {
    // Force Fields anwenden
    foreach(var forceField in _forceFields) {
        forceField.ApplyForce(particle, deltaTime);
    }
    
    particle.Update(playerPosition, deltaTime);
    
    // GPU-Daten vorbereiten
    Vector4 currentColor = particle.GetCurrentColor();
    _particlePositionSizeData[4 * particleCount + 0] = particle.Position.X;
    _particlePositionSizeData[4 * particleCount + 1] = particle.Position.Y;
    _particlePositionSizeData[4 * particleCount + 2] = particle.GetCurrentSize();
    
    _particleColorData[4 * particleCount + 0] = (byte)(currentColor.X * 255);
    _particleColorData[4 * particleCount + 1] = (byte)(currentColor.Y * 255);
    
    // GPU Buffer Updates
    GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, 
                    particleCount * 4 * sizeof(float), _particlePositionSizeData);
}
```

## 6. Character & AI System

### Character Basis
**Hauptklasse**: `Core/world/character.cs:13-301`

```csharp
public class Character : Game_Object {
    public float movement_speed { get; set; } = 100.0f;
    public float health { get; set; }
    public float health_max { get; set; }
    public List<PowerUp> ActivePowerUps { get; set; } = new List<PowerUp>();
    public Ability Ability { get; set; }
    
    // Physics-based Movement
    public void add_force(Vec2 force) {
        if (this.collider != null && this.collider.body != null)
            this.collider.body.ApplyForce(force, Vec2.Zero);
    }
    
    // Damage System
    public virtual void apply_damage(float damage) {
        if (!Invincible) {
            health -= damage;
            if (health <= 0 && death_callback != null)
                death_callback();
        }
    }
}
```

### AI Perception System
**Code**: `Core/world/character.cs:144-163`

```csharp
public void perception_check(ref List<Game_Object> intersected_game_objects, 
                           float check_direction = 0, int num_of_rays = 6, 
                           float angle = float.Pi, float look_distance = 800) {
    float angle_per_ray = angle / (float)(num_of_rays - 1);
    for (int x = 0; x < num_of_rays; x++) {
        var look_dir = Core.util.util.vector_from_angle(transform.rotation - rotation_offset + 
                                                        check_direction - (angle / 2) + (angle_per_ray * x));
        Vector2 start = transform.position + (look_dir * (transform.size.X / 2));
        Vector2 end = start + (look_dir * look_distance);
        
        if (Game.Instance.get_active_map().ray_cast(start, end, out Vec2 intersection_point, 
                                                   out float distance, out var buffer)) {
            if (buffer != null && !intersected_game_objects.Contains(buffer))
                intersected_game_objects.Add(buffer);
        }
    }
}
```

### PowerUp System
**Code**: `Core/world/character.cs:169-191`

```csharp
public void add_power_up(PowerUp power_up) {
    if (all_power_ups.Contains(power_up)) return;
    
    all_power_ups.Add(power_up);
    power_up.activation(this);  // Dynamische Fähigkeiten-Anwendung
}

public void force_remove_power_up(PowerUp power_up) {
    if (!all_power_ups.Contains(power_up)) return;
    
    power_up.deactivation(this);
    all_power_ups.Remove(power_up);
}
```

### Ability System mit Cooldowns
**Code**: `Core/world/character.cs:195-213`

```csharp
public void UseAbility() {
    if(Game.Instance.player.Ability != null) {
        var currentTime = Game_Time.total;
        if (currentTime - abilityLastUsedTime >= Ability.Cooldown) {
            Ability.Use(this);
            abilityLastUsedTime = currentTime;
            
            if (Ability.Effect != null) {
                Ability.AddEffectToCharacter(this);
                Ability.Effect.Animation.Play();
            }
        }
    }
}
```

## 7. Physics-based Movement

### Newton'sche Physik Implementation

**Bewegung über Box2D Forces**: `Core/world/character.cs:111-116`
```csharp
public void add_force(Vec2 force) {
    if (this.collider != null && this.collider.body != null)
        this.collider.body.ApplyForce(force, Vec2.Zero);
}
```

**Velocity Control**: `Core/world/character.cs:97-109`
```csharp
public void Set_Velocity(Vec2 new_velocity) {
    if (this.collider != null && this.collider.body != null)
        this.collider.body.SetLinearVelocity(new_velocity);
}

public Vec2 Get_Velocity() {
    if (this.collider != null && this.collider.body != null)
        return this.collider.body.GetLinearVelocity();
    return Vec2.Zero;
}
```

### Character Physics Properties
**Code**: `Core/world/map.cs:161-163`
```csharp
BodyDef def = new();
def.LinearDamping = 1.0f;    // Reibung für natürliche Bremsung
def.AllowSleep = false;      // Characters schlafen nie ein
```

## 8. Map & World System

### Physics World Management
**Code**: `Core/world/map.cs:14-47`

```csharp
public class Map {
    public readonly World physicsWorld;
    private const int MaxPhysicsBodies = 400;
    
    public Map() {
        AABB aabb = new();
        aabb.LowerBound.Set(-100000, -100000);
        aabb.UpperBound.Set(100000, 100000);
        
        Vec2 gravity = new(0.0f, 0.001f);
        physicsWorld = new World(aabb, gravity, true);
        physicsWorld.SetContactListener(new CollisionListener());
    }
}
```

### Spatial Optimization mit Tiles
**Code**: `Core/world/map.cs:701-718`

```csharp
private Map_Tile Get_Correct_Map_Tile(Vector2 position) {
    int final_tileSize = tileSize * cellSize;
    Vector2i key = new((int)System.Math.Floor(position.X / final_tileSize), 
                       (int)System.Math.Floor(position.Y / final_tileSize));
    key *= final_tileSize;
    
    if(!mapTiles.ContainsKey(key)) {
        mapTiles.Add(key, new Map_Tile(key));
    }
    
    mapTiles.TryGetValue(key, out Map_Tile current_tile);
    return current_tile;
}
```

### Frustum Culling für Performance
**Code**: `Core/world/map.cs:542-563`

```csharp
internal void Draw() {
    Vector2 camera_pos = Game.Instance.camera.transform.position;
    Vector2 camera_size = Game.Instance.camera.Get_View_Size_In_World_Coord() + new Vector2(cellSize * 2);
    
    foreach(var tile in mapTiles) {
        // AABB-Overlap Test für Culling
        float overlapX = camera_size.X / 2 + tile_size / 2 - Math.Abs(camera_pos.X - tile.Key.X);
        float overlapY = camera_size.Y / 2 + tile_size / 2 - Math.Abs(camera_pos.Y - tile.Key.Y);
        if(overlapX > 0 && overlapY > 0) {
            // Nur sichtbare Tiles rendern
            foreach(var sprite in tile.Value.background)
                sprite.Draw();
        }
    }
}
```

## 9. State Management

### Game State System
**Code**: `Core/game.cs:19-32`

```csharp
public enum Play_State {
    main_menu = 0,
    Playing = 1,
    dead = 2,
    skill_tree = 3,
    ability_skill_tree = 4,
    powerup_skill_tree = 5,
    InGameMenu = 6,
    LevelUp = 7,
    PauseMenuSkillTree = 8,
    PauseAbilitySkillTree = 9,
    PausePowerupSkillTree = 10
}
```

### Game State Events
**Code**: `Core/game.cs:34-43`

```csharp
public class GameStateChangedEventArgs : EventArgs {
    public Play_State OldState { get; }
    public Play_State NewState { get; }
}

public event EventHandler<GameStateChangedEventArgs> GameStateChanged;
```

## 10. AI State Machine System

### Finite State Machine für Gegner
**Hauptklasse**: `Projektarbeit/characters/enemy/States/`

Das Spiel verwendet eine klassische State Machine für Gegner-KI mit verschiedenen Zuständen:

**Attack State**: `Projektarbeit/characters/enemy/States/Attack.cs:9-50`
```csharp
public class Attack : I_state<AI_Controller> {
    public Type execute(AI_Controller aiController, float delta_time) {
        Type nextState = typeof(Attack);
        
        foreach (Character character in aiController.characters) {
            if (character is CH_base_NPC npc) {
                npc.Attack();
                if (!npc.IsPlayerInAttackRange()) {
                    nextState = typeof(Pursue);  // Zu weit weg → verfolgen
                }
                if (npc.IsHealthLow()) {
                    nextState = typeof(Retreat);  // Wenig Leben → fliehen
                }
            }
        }
        return nextState;
    }
}
```

**Pursue State**: `Projektarbeit/characters/enemy/States/Pursue.cs:9-49`
```csharp
public class Pursue : I_state<AI_Controller> {
    public Type execute(AI_Controller aiController, float delta_time) {
        foreach (Character character in aiController.characters) {
            if (character is CH_base_NPC npc) {
                npc.Pursue();  // Spieler verfolgen
                if (npc.IsPlayerInAttackRange()) {
                    nextState = typeof(Attack);  // Nah genug → angreifen
                }
            }
        }
        return nextState;
    }
}
```

### AI-Verhalten Implementation
**Hauptklasse**: `Projektarbeit/characters/enemy/character/CH_base_NPC.cs:11-290`

**Schwarm-Verhalten (Separation)**:
```csharp
public (Vector2, float) CalculateSeparationForce() {
    float separationDistance = 80f + ((float)random.NextDouble() * 30f);
    Vector2 totalSeparationForce = Vector2.Zero;
    
    foreach (var other in Controller.characters) {
        if (other == this) continue;
        
        float distance = (other.transform.position - transform.position).Length;
        if (distance < separationDistance) {
            Vector2 separationDirection = transform.position - other.transform.position;
            separationDirection.NormalizeFast();
            
            // Exponentieller Kraftabfall mit Distanz
            float separationForceMagnitude = (float)Math.Exp(-distance / 20f) * maxSeparationForce;
            totalSeparationForce += separationDirection * separationForceMagnitude;
        }
    }
    
    return (totalSeparationForce, separationSpeed);
}
```

**Verfolgungs-Logik**:
```csharp
public virtual void Pursue() {
    if (!IsPlayerInProximity(StopDistance)) {
        Vector2 direction = GetDirectionToPlayer();
        ApplyForceInDirection(direction, PursueSpeed);
        CalculateAndApplySeparationForce();  // Verhindert Gegner-Überlappung
    }
}

protected Vector2 GetDirectionToPlayer() {
    return (Game.Instance.player.transform.position - transform.position).Normalized();
}
```

## 11. Wave & Spawning System

### Prozedurales Wave Generation
**Hauptklasse**: `Projektarbeit/Levels/Wave.cs:7-381`

**Wave Tracking System**:
```csharp
public class Wave : Game_Object {
    public int TotalEnemiesInWave { get; private set; }
    public int EnemiesDefeated { get; set; } = 0;
    public float WaveProgress => TotalEnemiesInWave > 0 ? 
        (float)EnemiesDefeated / TotalEnemiesInWave : 0f;
    
    private readonly List<Spawner> spawners;
    private bool Started = false;
    private bool Finished = false;
}
```

**Dynamisches Schwierigkeitssystem**:
```csharp
private static float CalculateDifficultyMultiplier(int waveNumber) {
    float baseMultiplier = 1.0f;
    float growthRate = 0.15f; // 15% Steigerung pro Wave
    float maxMultiplier = 10.0f; // Maximum bei 10x
    
    float multiplier = baseMultiplier + (waveNumber - 1) * growthRate;
    return Math.Min(multiplier, maxMultiplier);
}
```

**Wave Type Bestimmung**:
```csharp
private enum WaveType {
    Tutorial,    // Waves 1-10: Lernen der Mechaniken
    Standard,    // Normale Waves mit Balance
    Boss,        // Jede 10. Wave: Boss mit Support
    Challenge,   // Jede 25. Wave: Spezielle Herausforderung
    BulletHell   // Waves 51+: Maximale Intensität
}

private static WaveType DetermineWaveType(int waveNumber) {
    if (waveNumber <= 10) return WaveType.Tutorial;
    if (waveNumber % 10 == 0) return WaveType.Boss;
    if (waveNumber % 25 == 0) return WaveType.Challenge;
    if (waveNumber >= 51) return WaveType.BulletHell;
    return WaveType.Standard;
}
```

### Spawner System
**Hauptklasse**: `Projektarbeit/Levels/Spawner.cs:8-82`

**Reflection-basierte Enemy Creation**:
```csharp
public class Spawner : Game_Object {
    private Type enemyControllerType;
    
    public Type ControllerType {
        set {
            if (value.BaseType == typeof(AI_Controller)) {
                enemyControllerType = value;
            } else {
                throw new Exception("Invalid Type");
            }
        }
    }
    
    public override void Update(float delta) {
        if (Game_Time.total > startTime + (spawned * SpawnRate)) {
            spawned++;
            // Dynamische Controller-Instanziierung über Reflection
            AI_Controller controller = (AI_Controller)Activator.CreateInstance(
                ControllerType, transform.position);
            Core.Game.Instance.get_active_map().add_AI_Controller(controller);
        }
    }
}
```

## 12. Ability System

### Ability Framework
**Basisklasse**: `Core/defaults/Ability.cs`

**Shield Ability Implementation**: `Projektarbeit/characters/player/abilities/ShieldAbility.cs:8-93`
```csharp
public class ShieldAbility : Ability {
    private readonly Timer timer;
    
    public override void Use(Character character) {
        this.character = character;
        character.Invincible = true;  // Unverwundbarkeit aktivieren
        
        AddEffectToCharacter(character);  // Visueller Effekt
        Core.Game.Instance.get_active_map().Add_Game_Object(Effect);
        
        timer.Interval = Duration * 1000;
        timer.Start();  // Auto-Deaktivierung nach Duration
    }
    
    private void OnTimerElapsed(object sender, ElapsedEventArgs e) {
        if (character != null) {
            character.Invincible = false;
            Core.Game.Instance.get_active_map().Remove_Game_Object(Effect);
        }
    }
    
    public override void Upgrade() {
        base.Upgrade();
        Duration += 0.5f;  // Längere Wirkungsdauer bei Upgrade
    }
}
```

### Cooldown System
**Code**: `Core/world/character.cs:195-213`
```csharp
public void UseAbility() {
    if(Game.Instance.player.Ability != null) {
        var currentTime = Game_Time.total;
        if (currentTime - abilityLastUsedTime >= Ability.Cooldown) {
            Ability.Use(this);
            abilityLastUsedTime = currentTime;
            
            if (Ability.Effect != null) {
                Ability.AddEffectToCharacter(this);
                Ability.Effect.Animation.Play();
            }
        }
    }
}
```

## 13. Power-Up System

### Power-Up Framework
**Health Power-Up**: `Projektarbeit/characters/player/power_ups/Health.cs:9-50`
```csharp
public class HealthIncrease : PowerUp {
    public HealthIncrease(Vector2 position)
        : base(position, new Vector2(30, 30), 
               new Sprite(new Texture("assets/textures/power-ups/health.png"))) {
        activation = ActivatePowerUp;
        deactivation = (target) => { };  // Sofortiger Effekt, keine Deaktivierung
        
        Name = "HealthIncrease";
        UnlockCost = 30;
        UpgradeMultiplier = 1.7f;
        HealthIncreaseAmount = 30f;
        Duration = 0f; // Instant effect
    }
    
    private void ActivatePowerUp(Character target) {
        if (target is CH_player player) {
            player.health += HealthIncreaseAmount;
            if (player.health > player.health_max) {
                player.health = player.health_max;  // Cap bei Maximum
            }
        }
    }
}
```

### Power-Up Upgrade System
```csharp
public override void Upgrade() {
    base.Upgrade();
    HealthIncreaseAmount += 5;  // Mehr Heilung pro Level
    Console.WriteLine($"HealthIncrease upgraded to level {Level}");
}
```

## 14. Spezielle Partikel-Systeme

### XP Partikel mit intelligenter Anziehung
**Hauptklasse**: `Projektarbeit/particles/XPParticle.cs:15-144`

**State Machine für XP Orbs**:
```csharp
public enum XPParticleState {
    Waiting,   // Noch nicht nah genug für Anziehung
    Attracted, // Bewegt sich zum Spieler
    Collected  // Zum Entfernen markiert
}

public override void Update(Vector2 playerPosition, float deltaTime) {
    Vector2 toPlayer = Game.Instance.player.transform.position - Position;
    float distance = toPlayer.Length;
    
    switch (State) {
        case XPParticleState.Waiting:
            if (distance <= AttractDistance) {
                State = XPParticleState.Attracted;
            }
            break;
            
        case XPParticleState.Attracted:
            // Intelligente Verfolgung mit Vorhersage
            Vector2 playerVelocity = util.convert_Vector<Vector2>(Game.Instance.player.Get_Velocity());
            Vector2 predictedPlayerPos = Game.Instance.player.transform.position + 
                                       playerVelocity * deltaTime * 2.0f;
            
            // Kraftanpassung basierend auf Distanz
            float forceFactor = 1.0f + (1.0f - (distance / AttractDistance)) * 2.0f;
            if (distance < AttractDistance * 0.3f) {
                forceFactor *= 2.0f; // Doppelte Kraft wenn sehr nah
            }
            
            Velocity += finalDirection * MaxAttractForce * forceFactor * deltaTime;
            break;
    }
}
```

### XP Partikel Generation nach Typ
**Code**: `Projektarbeit/particles/XPParticleEffect.cs:15-138`
```csharp
public enum XPOrbType {
    Small,   // Blau: 3-5 Pixel
    Medium,  // Lila: 5-8 Pixel  
    Large,   // Magenta: 8-12 Pixel
    Super    // Gold: 13-18 Pixel
}

public static void CreateByXP(ParticleSystem particleSystem, int xpValue, Vector2 position) {
    // Automatische Typ-Auswahl basierend auf XP-Wert
    if (xpValue <= 10)
        CreateByType(particleSystem, Math.Max(1, xpValue), position, XPOrbType.Small);
    else if (xpValue <= 30)
        CreateByType(particleSystem, Math.Max(1, xpValue / 2), position, XPOrbType.Medium);
    else if (xpValue <= 80)
        CreateByType(particleSystem, Math.Max(1, xpValue / 5), position, XPOrbType.Large);
    else
        CreateByType(particleSystem, Math.Max(1, xpValue / 10), position, XPOrbType.Super);
}
```

## 15. Level & Game Management

### Hauptspiel Loop
**Hauptklasse**: `Projektarbeit/Game.cs:13-195`

**State-based UI Rendering**:
```csharp
protected override void Render_Imgui(float deltaTime) {
    switch (play_state) {
        case Play_State.main_menu:
            mainMenu.Render();
            break;
        case Play_State.Playing:
            mainHUD.Render();
            break;
        case Play_State.dead:
            gameOver.Render();
            break;
        case Play_State.LevelUp:
            levelUpMenu.Render();
            break;
    }
}
```

**Power-Up/Ability Loading über Reflection**:
```csharp
public override List<PowerUp> loadPowerups(List<PowerUpSaveData> PowerUpsSaveData) {
    List<PowerUp> powerUps = new List<PowerUp>();
    foreach (var powerUpSaveData in PowerUpsSaveData) {
        PowerUp newPowerUp = null;
        switch(powerUpSaveData.PowerUpType) {
            case "FireRateBoost":
                newPowerUp = new FireRateBoost(new Vector2(999, 999), 
                    fireDelayDecrease: 0.1f, duration: 4f);
                break;
            case "HealthIncrease":
                newPowerUp = new HealthIncrease(new Vector2(999, 999));
                break;
        }
        
        if (newPowerUp != null) {
            newPowerUp.LoadFromSaveData(powerUpSaveData);
            powerUps.Add(newPowerUp);
        }
    }
    return powerUps;
}
```

### Game State Progression
```csharp
protected override void Update(float deltaTime) {
    if (GameState.AccountXP >= GameState.XPForNextLevel()) {
        GameState.IncreaseLevel();  // Automatisches Level-Up
    }
    GameState.RemoveDuplicatePowerUps();  // Duplikat-Bereinigung
}
```

## 16. Performance Optimierungen

### Debug & Profiling System
**Code**: `Core/game.cs:45-73`
```csharp
public static class DebugData {
    public static double workTimeUpdate = 0;
    public static double workTimeRender = 0;
    public static int spriteDrawCallsNum = 0;
    public static int colidableObjectsStatic = 0;
    public static int colidableObjectsDynamic = 0;
}
```

### Garbage Collection Management
**Code**: `Core/world/map.cs:677-684`
```csharp
if(use_garbage_collector) {
    if(physicsWorld.GetBodyCount() > MaxPhysicsBodies) {
        Console.WriteLine($"WARNING: Physics body count exceeded limit ({MaxPhysicsBodies}). Resetting physics world.");
        ResetPhysicsWorld();
    }
}
```

### Instanced Rendering für Partikel
- Bis zu 100.000 Partikel gleichzeitig
- Multi-Pass Rendering (Standard + Additive Blending)
- GPU-basierte Datenstrukturen

---

## Wichtige Prüfungskonzepte Zusammengefasst:

1. **Game Loop**: OpenTK Events mit Frame Rate Independence über deltaTime
2. **Kollision**: Box2D mit Broad/Narrow Phase, Raycast für AI
3. **Transformationen**: Hierarchische Matrix-Systeme, SRT-Order, Camera Shake
4. **Rendering**: Modern OpenGL Core Profile, VAO/VBO, Shader-Pipeline
5. **Partikelsystem**: Instanced Rendering, Force Fields, Multi-Pass, intelligente XP-Orbs
6. **Character System**: Physics-based Movement, PowerUps, Abilities mit Cooldowns
7. **AI System**: Finite State Machine, Schwarm-Verhalten, Separation Forces
8. **Wave System**: Prozedurales Generation, dynamische Schwierigkeit, Boss/Challenge Waves
9. **Ability System**: Timer-basiert, Upgrade-fähig, visuelle Effekte
10. **Performance**: Frustum Culling, Spatial Partitioning, Object Pooling, Physics Body Limits