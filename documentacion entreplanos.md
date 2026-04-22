# 1. Control del Jugador (`FirstPersonController`)

## Qué problema resuelve
Unity no tiene un controlador FPS listo para usar. Este script implementa un **sistema de movimiento kinemático** (no usa físicas de Rigidbody, sino que mueve al personaje manualmente frame a frame).

## Mecánica Interna
El script usa `CharacterController` en lugar de `Rigidbody`. Esto es una decisión técnica importante: `CharacterController` es un componente especial de Unity diseñado para personajes que se mueven con código pero necesitan respetar colisiones sin depender de la simulación física. No es afectado por fuerzas externas automáticamente.

### Flujo del Update
1. **Verificación de controles**: Si `controlsEnabled` es `false`, el script retorna inmediatamente. Esto evita procesar input innecesario cuando el jugador está en un keypad o menú.
2. **Modo de movimiento**: Lee `cameraSwitcherScript.movement`. Si es `Movement3D`, ejecuta `HandleMouseLook()` y `HandleMovement()`. Esto permite al juego alternar entre mecánicas 2D y 3D sin cambiar de escena.
3. **Mouse Look**:
   - Lee `Input.GetAxis("Mouse X")` y `Mouse Y`. Estos ejes mapean automáticamente el movimiento del mouse.
   - **Yaw**: Rota el transform del jugador en el eje Y (`transform.Rotate(Vector3.up * mouseX)`). Esto gira el cuerpo.
   - **Pitch**: Modifica `cameraPitch` restando `mouseY`, luego lo restringe con `Mathf.Clamp` entre `-maxLookAngle` y `maxLookAngle`. Evita que la cámara de vueltas completas verticales.
   - Si `smoothLook` está activo, usa `Quaternion.Slerp` para interpolar suavemente la rotación local de la cámara. Si no, aplica la rotación directamente.
4. **Movimiento**:
   - Obtiene los ejes Horizontal (`A/D`) y Vertical (`W/S`).
   - Si `moveRelativeToCamera` es true, proyecta los vectores `forward` y `right` de la cámara sobre el plano XZ (pone `.y = 0` y normaliza). Esto hace que "W" siempre signifique "hacia adelante desde la vista de la cámara", no "hacia el norte del mundo".
   - Normaliza el vector de movimiento si su magnitud al cuadrado supera 1. Esto evita que moverse en diagonal sea más rápido.
   - Detecta si se presiona Left Shift para alternar entre `walkSpeed` y `runSpeed`.
5. **Gravedad y Salto**:
   - Si `cc.isGrounded` es true (el CharacterController toca el suelo), resetea la velocidad vertical a `-1f` (un pequeño valor negativo para mantenerlo pegado al suelo).
   - Si se presiona el botón de salto (`Input.GetButtonDown("Jump")`), asigna `jumpSpeed` a `verticalVelocity`.
   - Si está en el aire, aplica gravedad acumulativa: `verticalVelocity -= gravity * Time.deltaTime`.
6. **Sonidos de pasos**:
   - Reproduce un `AudioSource` en bucle (`loop = true`) solo cuando se mantiene `W` y está en el suelo. Se detiene al saltar o soltar la tecla.

## Decisiones de diseño
- **¿Por qué CharacterController y no Rigidbody?** Porque un FPS necesita movimiento preciso y responsivo. Un Rigidbody introduce inercia, rebotes y problemas con rampas. CharacterController te da control total.
- **¿Por qué clamp en pitch y no en yaw?** Porque rotar 360° horizontalmente es natural, pero rotar verticalmente más de 90° rompe la inmersión.
- **¿Por qué Time.deltaTime?** Para hacer el movimiento independiente del framerate. Sin esto, el jugador correría más rápido a 144fps que a 30fps.

## Puntos clave
- `EnableControls(bool)` no solo cambia una variable booleana; también modifica `Cursor.lockState` y `Cursor.visible`. Esto es crítico porque el cursor del sistema operativo debe liberarse para interactuar con UI de Unity (como botones de keypad).
- El `footstepAudioSource` se configura en `Start()` con `loop = true`, pero se controla manualmente con `.Play()` y `.Stop()` en lugar de depender de eventos de animación.

---

# 2. Sistema de Interacción (`InteractWithObjects`, `IPickable`, `Key`)

## Arquitectura
El sistema usa una mezcla de **detección por volumen (triggers)** y **detección por rayo (SphereCast)**.

### InteractWithObjects.cs
Este script es el **orquestador**. Tiene dos modos de detección que funcionan en paralelo:

#### A. Detección de puertas por trigger
- Tiene un `OnTriggerEnter` y `OnTriggerExit` (requiere un Collider en el jugador marcado como trigger).
- Cuando el jugador entra en una puerta etiquetada (`LockedDoor`, `Door`, `CodeDoor`), activa `interactionWithDoor = true` y guarda la referencia al GameObject.
- En `Update`, si `interactionWithDoor` es true y se hace clic izquierdo (`Input.GetMouseButtonDown(0)`), usa `GetComponent` para identificar qué tipo de puerta es:
  - **`CodeDoorBehavior`**: Activa el GameObject del keypad, habilita un Canvas, desbloquea el cursor y llama a `firstPersonController.EnableControls(false)`.
  - **`DoorController`**: Verifica si está desbloqueada. Si no, busca componentes `Key` en los hijos de la cámara (`Camera.GetComponentsInChildren<Key>()`). Esto es importante: el inventario no es una lista abstracta, sino que las llaves existen físicamente como hijos del jugador en la jerarquía de Unity.
  - **`NormalDoorBehavior`**: Simplemente llama `Toggle()`.

#### B. Recogida de objetos por SphereCast
- Usa `Physics.SphereCast` en lugar de `Raycast`. Un SphereCast es como un rayo con grosor: lanza una esfera a lo largo de una dirección.
- Origen: `pickUpOrigin.transform.position`.
- Dirección: `pickUpOrigin.transform.TransformDirection(Vector3.forward)` (el forward local del origen).
- Radio: `pickUpRadius`.
- Si golpea algo, busca el componente `IPickable` y llama `OnPickUp(gameObject)`.

**¿Por qué SphereCast y no Raycast?** Porque un rayo finito es difícil de apuntar en primera persona. Una esfera es más permisiva y se siente mejor al jugar.

### IPickable.cs
Una **interfaz C#** (contrato). Cualquier clase que la implemente garantiza que tiene `OnPickUp`, `OnDrop` y `IsPicked`. Esto permite que `InteractWithObjects` no conozca qué objeto específico es; solo sabe que es "recogible".

### Key.cs
Implementa `IPickable` y maneja dos estados físicos distintos:

#### Estado Recogido (`OnPickUp`):
1. `isPicked = true`.
2. Encuentra el transform de la cámara del jugador: `picker.GetComponentInChildren<Camera>()?.transform ?? picker.transform`.
3. Se hace hijo de esa cámara: `transform.SetParent(holdParent, worldPositionStays: false)`. El segundo parámetro `false` significa que se reinicia la escala/rotación local relativa al nuevo padre.
4. Posición local fija: `transform.localPosition = holdLocalPosition` (definido como `(0, -0.25, 0.8)` en `Awake`).
5. Desactiva física: `rb.isKinematic = true` (el motor de física deja de moverlo).
6. Desactiva collider: `col.enabled = false` (ya no choca con nada).

#### Estado Soltado (`OnDrop`):
1. `transform.SetParent(null)` (se desvincula de la cámara).
2. Calcula una posición de caída: posición del padre + `forward * dropDistance`.
3. Reactiva física: `rb.isKinematic = false`, resetea velocidad.
4. Reactiva collider.

## Decisiones de diseño
- El inventario es **hierárquico**, no una estructura de datos en memoria. Las llaves existen como objetos en la escena, simplemente reubican su posición en el árbol de transforms. Esto es simple pero limitado (no escala bien a inventarios grandes).
- El uso de `GetComponentInChildren<Camera>()` asume que hay una cámara dentro del jugador, lo cual es estándar en FPS.

---

# 3. Sistema de Puertas

Hay tres tipos de puertas. Todas comparten la misma mecánica de movimiento pero difieren en sus reglas de acceso.

### Mecánica compartida: Corrutinas e Interpolación
Todas usan `IEnumerator ToggleDoor(bool open)` (o similar). Una **corrutina** es una función que puede pausarse (`yield return null`) y reanudarse en el siguiente frame.

#### Algoritmo de movimiento
1. Guarda la posición y rotación actuales (`fromPos`, `fromRot`).
2. Determina el destino (`toPos`, `toRot`) basado en el parámetro `open`.
3. Inicializa `t = 0f`.
4. En un bucle `while (t < 1f)`:
   - Incrementa `t` en `Time.deltaTime * doorSpeed`.
   - `transform.position = Vector3.Lerp(fromPos, toPos, t)`: interpola linealmente entre el origen y el destino.
   - Si `lerpRotation` es true, `transform.rotation = Quaternion.Lerp(fromRot, toRot, t)`: interpola rotaciones usando cuaternios (evita el problema del gimbal lock de los Euler angles).
   - `yield return null`: cede el control hasta el siguiente frame.
5. Al salir del bucle, fuerza los valores finales exactos para evitar errores de precisión.

**¿Por qué Lerp manual y no Animations?** Porque las puertas necesitan ser interrumpidas y controladas desde código en tiempo real (por ejemplo, `ScaryEvent_1` cambia la velocidad de la puerta dinámicamente).

### DoorController.cs (Puerta Bloqueada)
- Tiene un estado `isUnlocked` que actúa como guardia. Ninguna función de apertura funciona sin pasar por `Unlock()`.
- `Unlock()` reproduce un sonido y cambia el estado.
- Usa **posiciones absolutas** para abierto/cerrado pero preserva la Y original. Esto permite colocar la puerta en cualquier altura sin que el diseñador tenga que recalcular las coordenadas.

### NormalDoorBehavior.cs (Puerta Normal)
- Similar a `DoorController` pero sin llave. Añade `isLocked` como bloqueo adicional.
- Los métodos `Open`, `Close` y `Toggle` permiten control externo (útil para eventos scripteados).
- El parámetro `playSound` en `ToggleDoor` permite que eventos como `ScaryEvent_1` supriman los sonidos normales y usen los suyos propios.

### CodeDoorBehavior.cs (Puerta con Código)
- Es la más compleja porque mediate entre el mundo 3D y una UI 2D.
- `Toggle()` no abre la puerta directamente; primero verifica `keypad.openTheDoor`.
- Si el código no está resuelto, cierra la UI del keypad (`CloseKeyPad()`) y restaura controles.
- `CloseKeyPad()` desactiva el GameObject del keypad, bloquea el cursor (`CursorLockMode.Locked`) y reactiva controles del jugador. Esto es una **transición de estado UI→Gameplay** crítica.

### KeyPad.cs (Lógica del Teclado Numérico)
- Usa `TMP_Text` para mostrar números.
- `Number(int)` limita la entrada a 4 caracteres verificando `Ans.text.Length < 4`.
- `Execute()` compara cadenas exactas (`Ans.text == answer`). Si coinciden, notifica a `CodeDoorBehavior` y restaura controles via `FindFirstObjectByType<InteractWithObjects>()`.

## Puntos clave técnicos
- **Interpolación no acelerada**: El Lerp se hace con `t` que crece linealmente (`t += Time.deltaTime * speed`). Esto produce movimiento a velocidad constante. No se usa `Mathf.SmoothStep` aquí, a diferencia de `CameraMovementMenu`.
- **Estados concurrentes**: `isAnimating` evita que se inicien múltiples corrutinas simultáneas. Sin esto, presionar el botón dos veces haría que la puerta intentara abrirse y cerrarse al mismo tiempo.

---

# 4. Sistema de Enemigos

### Enemy.cs (A* Pathfinding)
Usa el asset externo **A* Pathfinding Project** (no el NavMesh integrado de Unity).

#### Componentes técnicos
- `AIPath`: Componente del asset que calcula rutas automáticamente hacia un destino.
- `path.destination`: Vector3 objetivo. Se actualiza cada frame a la posición del jugador.
- `path.maxSpeed`: Velocidad máxima permitida para el agente.

#### Lógica de Update
1. Calcula `distanceToTarget = Vector3.Distance(transform.position, target.position)`.
2. Si la distancia es menor que `stoppingDistance`, establece el destino a la posición actual (`transform.position`). Esto detiene al agente.
3. Si no, actualiza el destino al jugador.
4. Obtiene la velocidad actual del agente: `path.velocity` (un Vector2 o Vector3 según la configuración 2D/3D del asset).
5. Pasa esa velocidad al animator como parámetros `XMovement`, `YMovement` e `IsMoving`.

**Nota técnica**: A diferencia de NavMeshAgent, `AIPath` maneja su propio sistema de grids o grafos de nodos. No depende de la geometría de navegación bakeada de Unity.

### EnemyBehavior.cs (NavMeshAgent de Unity)
Usa el sistema nativo de Unity.

#### Lógica de Update
1. Calcula distancia al jugador.
2. Si `distance < rangeToChase`, `isChasing = true`.
3. Si está persiguiendo, asigna `EnemyMeshAgent.SetDestination(player.position)` y ajusta la velocidad.
4. Si no está persiguiendo, velocidad es 0.

**Diferencia clave con Enemy.cs**: Este sistema requiere que la escena tenga un **NavMesh bakeado** (navegación horneada) desde la ventana AI > Navigation. `Enemy.cs` con A* no lo necesita si usa grid graphs.

#### Gizmos
`OnDrawGizmos` dibuja una esfera de alambre roja en la posición del agente con radio `rangeToChase`. Esto es solo visual en el editor.

---

# 5. Post-Procesamiento URP (Pixelation, Dithering, Fog)

Este es el sistema más complejo técnicamente. Implementa efectos de pantalla completa en el **Universal Render Pipeline** usando la API de Volúmenes y Render Features.

## Conceptos previos de Unity URP
- **Volume**: Un sistema que permite mezclar parámetros de post-proceso en zonas del mundo.
- **VolumeComponent**: Un scriptable object que define qué parámetros tiene un efecto.
- **ScriptableRendererFeature**: Un plugin para el renderer que te permite inyectar pasos de renderizado personalizados.
- **ScriptableRenderPass**: Una operación de renderizado que ocurre en un momento específico del pipeline.
- **CommandBuffer**: Una lista de comandos de GPU que se ejecutan de una sola vez.
- **Blit**: Copiar una textura a otra, opcionalmente aplicando un material/shader.

## Arquitectura de cada efecto (todos siguen el mismo patrón)

### Paso 1: Definición de parámetros (Pixelation.cs, Dithering.cs, Fog.cs)
Heredan de `VolumeComponent` e implementan `IPostProcessComponent`.

- `VolumeComponent` les permite vivir dentro de un perfil de Volume.
- Los parámetros no son `float` simples; son `FloatParameter`, `ColorParameter`, etc. Estos envuelven el valor real y permiten que el sistema de Volúmenes haga interpolación (override blending) entre diferentes zonas.
- `IsActive() => true` significa que el efecto siempre está técnicamente activo.
- `IsTileCompatible() => false` indica que el efecto no puede ejecutarse en el sistema de tiles de URP (porque modifica la pantalla completa).

### Paso 2: Controlador (PixelationController.cs, etc.)
Un MonoBehaviour que conecta valores del Inspector con el Volume Profile.

- En `Update`, llama a `SetParams()`.
- Busca el componente en el perfil: `volumeProfile.TryGet<Pixelation>(out this.pixelation)`.
- Si lo encuentra, copia los valores locales a las propiedades del VolumeComponent.
- `[ExecuteInEditMode]` hace que esto funcione en el editor sin estar en Play mode.

### Paso 3: Render Feature (PixelationRenderFeature.cs, etc.)
Hereda de `ScriptableRendererFeature`.

#### `Create()`
Instancia la pasada personalizada:
```csharp
pixelationPass = new PixelationPass(RenderPassEvent.BeforeRenderingPostProcessing);
```
`BeforeRenderingPostProcessing` significa que se ejecuta justo antes de que URP aplique los efectos de post-proceso integrados (bloom, DOF, etc.).

#### `AddRenderPasses()`
Añade la pasada a la cola del renderer:
```csharp
renderer.EnqueuePass(pixelationPass);
```

#### `SetupRenderPasses()`
Configura el objetivo de renderizado (el color buffer de la cámara):
```csharp
pixelationPass.Setup(renderer.cameraColorTargetHandle);
```

### Paso 4: Render Pass (PixelationPass, etc.)
Hereda de `ScriptableRenderPass`.

#### Constructor
- Busca el shader por nombre: `Shader.Find("PostEffect/Pixelation")`.
- Crea un Material a partir de ese shader: `CoreUtils.CreateEngineMaterial(shader)`.
- Si el shader no existe, loggea error.

#### `Execute()`
Este es el corazón. Se llama cada vez que Unity renderiza un frame.

1. Verifica que el material exista y que la cámara tenga post-procesado habilitado (`renderingData.cameraData.postProcessEnabled`).
2. Obtiene la pila de volúmenes activos: `VolumeManager.instance.stack`.
3. Extrae el componente de efecto: `stack.GetComponent<Pixelation>()`.
4. Obtiene un `CommandBuffer` del pool: `CommandBufferPool.Get(k_RenderTag)`. Esto es eficiente porque recicla buffers en lugar de crear nuevos cada frame.
5. Llama a `Render(cmd, ref renderingData)`.
6. Ejecuta el buffer: `context.ExecuteCommandBuffer(cmd)`.
7. Libera el buffer: `CommandBufferPool.Release(cmd)`.

#### `Render()`
1. Obtiene las dimensiones de la cámara: `cameraData.camera.scaledPixelWidth/Height`.
2. Activa la textura de profundidad: `cameraData.camera.depthTextureMode |= DepthTextureMode.Depth`. Algunos shaders de post-proceso necesitan la profundidad para calcular distancias.
3. Asigna los parámetros al material usando IDs numéricas (más rápido que strings):
   ```csharp
   this.pixelationMaterial.SetFloat(WidthPixelation, this.pixelation.widthPixelation.value);
   ```
4. Crea una textura temporal: `cmd.GetTemporaryRT(destination, w, h, 0, FilterMode.Point, RenderTextureFormat.Default)`. Es temporal porque solo existe durante este frame.
5. Hace dos Blits:
   - Primero copia el frame actual a la textura temporal.
   - Luego copia de la textura temporal de vuelta al buffer de cámara, aplicando el material con el shader.

**¿Por qué dos Blits?** Porque no puedes leer y escribir la misma textura simultáneamente en GPU. Necesitas un buffer intermedio.

## Decisiones de diseño
- **FilterMode.Point**: Asegura que el pixelación sea nítida (sin interpolación bilineal), manteniendo el estilo retro PS1.
- **PropertyToID**: Usar IDs enteros para propiedades de shader es una optimización estándar. Las strings se hashean cada vez; los IDs son lookups O(1).

---

# 6. Menú, Guardado y Transiciones

### SaveSystem.cs
Una clase `static`. No necesita instanciarse.

#### Mecanismo de serialización
Usa `PlayerPrefs`, que es un almacenamiento clave-valor persistente en el disco (registro en Windows, plist en macOS, SharedPreferences en Android).

- Claves: `"Save_Scene"`, `"Save_PosX"`, `"Save_PosY"`, `"Save_PosZ"`.
- `PlayerPrefs.Save()` fuerza la escritura inmediata al disco. Sin esto, Unity podría retrasar el guardado.

**Limitación técnica**: `PlayerPrefs` no está diseñado para juegos complejos. Solo almacena floats, ints y strings. No tiene cifrado, límite de tamaño razonablemente grande (~1MB), y es fácil de editar externamente. Para un juego pequeño es suficiente.

### SavePoint.cs
- En `Reset()` (se llama al agregar el componente en el editor), verifica si tiene collider. Si no, agrega un `BoxCollider` y lo marca como trigger. Esto es una conveniencia para diseñadores.
- `OnTriggerEnter` detecta al jugador por tag y llama a `SaveSystem.SavePlayerPosition()`.

### MenuSystem.cs
#### `Continue()`
1. Determina la escena a cargar. Si hay guardado, usa `SaveSystem.GetSavedScene()`. Si no, usa `"MainScene"`.
2. Llama a `vhsTransition.PlayTransition(sceneToLoad)`.
3. Se suscribe al evento `SceneManager.sceneLoaded += OnSceneLoaded_SetPlayer`. Esto es una **callback asincrónica**: no sabes cuándo terminará la carga, así que registras una función que se ejecutará automáticamente cuando la escena esté lista.
4. `OnSceneLoaded_SetPlayer` se desuscribe inmediatamente (`SceneManager.sceneLoaded -= ...`) para evitar que se llame múltiples veces. Luego reposiciona al jugador.

### VHSTransition.cs
Gestiona una transición entre escenas con efectos visuales.

#### `Awake()`
`DontDestroyOnLoad(gameObject)` evita que el objeto se destruya al cambiar de escena. Esto es esencial porque la transición debe persistir mientras se carga la nueva escena.

#### `PlayTransition()`
Activa/desactiva canvases y arranca `DoTransition()`.

#### `DoTransition()` (corrutina)
1. **Fase de entrada**: Aumenta `time` desde 0 hasta 1 usando `Time.deltaTime * transitionSpeed`.
   - Aplica `FilmGrain` con `Mathf.PingPong(Time.time * 2f, grainIntensityMax)`. PingPong oscila entre 0 y el máximo, creando un efecto de grano intermitente.
   - Aplica `ChromaticAberration` con otra frecuencia de PingPong.
2. **Carga asíncrona**: `SceneManager.LoadSceneAsync(sceneName)` no congela el juego. Retorna un `AsyncOperation`.
   - Espera en un bucle `while (!asyncLoad.isDone)` cediendo cada frame.
3. **Fase de salida**: Reduce `time` desde 1 hasta 0, desvaneciendo los efectos.
4. **Limpieza**: Resetea todos los valores del volumen a 0 para no contaminar la nueva escena.

**Problema técnico conocido**: El script no destruye el objeto después de la transición. Como tiene `DontDestroyOnLoad`, persistirá para siempre si no se destruye manualmente. Esto podría causar duplicados si vuelves al menú.

---

# 7. Cámaras y Movimiento en Menú

### CameraMovementMenu.cs
Implementa un **sistema de spline/waypoints** para recorridos cinematográficos.

#### `PlaybackRoutine()` (corrutina)
Usa un bucle `do { ... } while (loop)` para repetición infinita si `loop` es true.

Para cada waypoint:
1. Guarda posición/rotación de inicio.
2. Calcula el punto a mirar:
   - Si `lookAtNextWaypoint` es true, mira al waypoint siguiente (usando módulo `%` para loop).
   - Si no, adopta la rotación del waypoint actual.
3. Bucle de interpolación `while (elapsed < duration)`:
   - `Mathf.SmoothStep(0f, 1f, t)` aplica una curva de aceleración/desaceleración suave (derivada cero en los extremos).
   - `Vector3.Lerp` para posición.
   - `Quaternion.Slerp` para rotación (interpolación esférica, la forma correcta de rotar entre orientaciones).
4. Espera `waitAtWaypoint` segundos.

#### Uso de RenderTexture
- La cámara del menú no renderiza a pantalla. Renderiza a una `RenderTexture`.
- Esa textura se asigna a un `RawImage` en UI (`backgroundRawImage.texture = renderTexture`).
- Esto permite tener el menú UI convencional sobre un fondo en movimiento 3D.

---

# 8. Eventos y Mundo

### ScaryEvent_1.cs
Un **secuenciador de eventos** basado en corrutinas.

#### Flujo técnico
1. `OnTriggerEnter` con el jugador activa `eventoActivado = true` (bandera de una sola vez).
2. Apaga la luz (`light.SetActive(false)`).
3. Teletransporta la lámpara a posición/rotación final (simulando que cayó).
4. Inicia `CerrarYPonerRapida()`.

#### Dentro de la corrutina
- Modifica propiedades directamente en `NormalDoorBehavior`: cambia `doorSpeed` y `isLocked`.
- Llama `yield return StartCoroutine(puerta.ToggleDoor(false, false))`. **Importante**: espera a que la puerta termine de cerrarse antes de continuar. El `false` en sonido suprime el audio normal.
- Reproduce sonidos secuenciales usando `audioSource.PlayOneShot()` y esperando sus duraciones con `yield return new WaitForSeconds(...)`.
- Para sonidos largos (`cryingSound`, `runingSound`), usa `audioSource.clip` y `Play()` en lugar de `PlayOneShot()` para poder controlarlos mejor, aunque luego los deja terminar solos.
- Al final, restaura la velocidad original y desbloquea la puerta.

## Puntos clave
- Este script demuestra **acoplamiento directo**: conoce detalles internos de `NormalDoorBehavior` (como `doorSpeed` y `isLocked`). Si la API de la puerta cambia, este script se rompe.
- La secuencia es **lineal e irreversible**. No hay forma de interrumpirla una vez iniciada.

### RoomChanguer.cs
Sistema de transición de habitaciones 2D.

- `OnTriggerEnter2D` detecta colisiones en 2D (requiere Collider2D).
- Usa un `switch` sobre el enum `Direction` para decidir el desplazamiento.
- Para `Left`/`Right`, mueve al jugador ±2 unidades en X y mueve la cámara ±20 unidades en X via `CameraPosChanguer.ChangeCameraPos()`.

### CameraPosChanguer.cs
Literalmente un wrapper de `transform.position = newPos`. Su valor es puramente organizativo: da un nombre semántico a la operación de cambiar la cámara.

---

# 9. UI y Diálogo

### Dialogue.cs
Implementa una **máquina de estados simple** para conversaciones.

#### Estados implícitos
1. **Idle**: Esperando al jugador (`isDialogueStarted = false`).
2. **Typing**: Escribiendo texto letra por letra.
3. **Waiting**: Texto completo mostrado, esperando input para siguiente línea.

#### Lógica de Update
- Si el jugador presiona `E`:
  - Si no empezó: `StartDialogue()`.
  - Si ya empezó y el texto actual es igual a la línea completa (`dialogueText.text == dialogueLines[lineIndex]`): avanza a `NextLine()`.
  - Si está en medio de escribir: `StopAllCoroutines()` detiene la corrutina de tipeo y muestra la línea completa inmediatamente.

#### `ShowLine()` (corrutina)
Itera sobre cada `char` en la cadena, concatenándolo al `TMP_Text` con un retraso de `typingTime` segundos por carácter.

**Ineficiencia técnica**: Concatenar strings con `+=` en un bucle genera garbage (basura para el GC) porque los strings son inmutables en C#. Para diálogos largos, sería mejor usar `StringBuilder`. Sin embargo, para un juego pequeño el impacto es mínimo.

#### Detección
Usa `OnTriggerEnter3D` y `OnTriggerExit3D`. **Nota**: El nombre correcto en Unity es `OnTriggerEnter`, no `OnTriggerEnter3D`. Parece que el código tiene un error tipográfico que probablemente no funciona. Los métodos correctos son `OnTriggerEnter(Collider)` y `OnTriggerExit(Collider)`. Si estos no se llaman, el diálogo nunca se activaría.

### UI_SpritesBehavior.cs
Gestiona la mira (crosshair) dinámicamente.

- Cachea `crosshairImage` en `Start()`.
- En `OnTriggerEnter`, detecta tags y asigna sprites.
- Para puertas con código, modifica la escala del `RectTransform` (de 10 a 20) para hacer la mira más grande.
- **Problema técnico**: En `OnTriggerExit` para `CodeDoor`, reasigna `CodeDoorSprite` pero no restaura el sprite por defecto. Además, no hay lógica para restaurar el sprite base cuando sales de `LockedDoor` o `Door` normales.

---

## Resumen de Problemas Técnicos Detectados (para tu conocimiento)

| Script | Problema | Impacto |
|---|---|---|
| `Dialogue.cs` | Usa `OnTriggerEnter3D` en lugar de `OnTriggerEnter` | El diálogo probablemente no se active |
| `UI_SpritesBehavior.cs` | No restaura el sprite por defecto al salir de triggers | La mira puede quedarse con el sprite incorrecto |
| `VHSTransition.cs` | No se destruye después de la transición | Acumulación de objetos DontDestroyOnLoad |
| `Dialogue.cs` | Concatenación de strings en bucle | Garbage collection innecesario |
| `Key.cs` | `FindPlayerInRange()` usa `FindWithTag` como fallback | Búsqueda costosa si se llama frecuentemente |

---

Si quieres, puedo profundizar aún más en algún sistema específico, como la matemática detrás del `Quaternion.Slerp` en las puertas, o cómo exactamente URP procesa el `ScriptableRenderPass`.