using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Aula1Setup
{
    private static readonly Color Gray = new Color(0.5f, 0.5f, 0.5f);
    private static readonly Color PlazaDarkGray = new Color(0.25f, 0.25f, 0.25f);
    private const string PlazaMaterialPath = "Assets/_Project/Art/Materials/Plaza_DarkGray.mat";

    [MenuItem("bip_project/Aula 1/Montar Greybox")]
    private static void MontarGreybox()
    {
        if (!PodeEditarCena())
            return;

        Scene scene = SceneManager.GetActiveScene();
        if (!scene.IsValid() || !scene.isLoaded)
        {
            Debug.LogError("Abra uma cena antes de montar o greybox.");
            return;
        }

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Montar Greybox da Aula 1");

        GameObject zona1 = CriarGrupo("Zona1_Vila", Vector3.zero);
        CriarPrimitiva(zona1.transform, PrimitiveType.Cube, "Chao_Vila",
            new Vector3(0f, -0.5f, 0f), new Vector3(40f, 1f, 40f));
        GameObject praca = CriarPrimitiva(zona1.transform, PrimitiveType.Cylinder, "Praca_Circular",
            new Vector3(0f, 0.1f, 0f), new Vector3(16f, 0.1f, 16f));
        ConfigurarColisorDaPlataforma(praca);
        AplicarMaterialPraca(praca);

        GameObject corredor = CriarGrupo("Corredor_Zona1_Zona2", new Vector3(0f, 0f, 30f));
        CriarPrimitiva(corredor.transform, PrimitiveType.Cube, "Piso_Corredor",
            new Vector3(0f, -0.5f, 0f), new Vector3(6f, 1f, 20f));

        GameObject zona2 = CriarGrupo("Zona2_Patio", new Vector3(0f, 0f, 55f));
        CriarPrimitiva(zona2.transform, PrimitiveType.Cube, "Chao_Patio",
            new Vector3(0f, -0.5f, 0f), new Vector3(50f, 1f, 30f));
        CriarMurosDoPatio(zona2.transform);
        CriarCaixas(zona2.transform);

        GameObject zona3 = CriarGrupo("Zona3_Forja", new Vector3(0f, 0f, 86f));
        GameObject arena = CriarPrimitiva(zona3.transform, PrimitiveType.Cylinder, "Arena_Circular",
            new Vector3(0f, 0.15f, 0f), new Vector3(30f, 0.15f, 30f));
        ConfigurarColisorDaPlataforma(arena);
        CriarPilares(zona3.transform);
        CriarLuzVermelha(zona3.transform);
        AjustarLuzDirecional();
        CriarLimitesInvisiveis(scene, zona3.transform);

        EditorSceneManager.MarkSceneDirty(scene);
        Undo.CollapseUndoOperations(undoGroup);
    }

    [MenuItem("bip_project/Aula 1/Montar Greybox", true)]
    private static bool ValidarMontarGreybox()
    {
        return !EditorApplication.isPlayingOrWillChangePlaymode;
    }

    [MenuItem("bip_project/Aula 1/Corrigir Acessos e Limites")]
    private static void CorrigirAcessosELimites()
    {
        if (!PodeEditarCena())
            return;

        Scene scene = SceneManager.GetActiveScene();
        GameObject zona1 = EncontrarRaiz(scene, "Zona1_Vila");
        GameObject zona3 = EncontrarRaiz(scene, "Zona3_Forja");
        Transform praca = zona1 != null ? zona1.transform.Find("Praca_Circular") : null;
        Transform arena = zona3 != null ? zona3.transform.Find("Arena_Circular") : null;
        if (praca == null || arena == null)
        {
            Debug.LogError("Nao encontrei a Praca_Circular e a Arena_Circular. Monte o greybox primeiro.");
            return;
        }

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Corrigir Acessos e Limites");

        AjustarPlataforma(praca, new Vector3(0f, 0.1f, 0f), new Vector3(16f, 0.1f, 16f));
        AjustarPlataforma(arena, new Vector3(0f, 0.15f, 0f), new Vector3(30f, 0.15f, 30f));
        ConfigurarColisorDaPlataforma(praca.gameObject);
        ConfigurarColisorDaPlataforma(arena.gameObject);
        AplicarMaterialPraca(praca.gameObject);
        CriarLimitesInvisiveis(scene, zona3.transform);

        EditorSceneManager.MarkSceneDirty(scene);
        Undo.CollapseUndoOperations(undoGroup);
    }

    [MenuItem("bip_project/Aula 1/Corrigir Acessos e Limites", true)]
    private static bool ValidarCorrigirAcessosELimites()
    {
        return !EditorApplication.isPlayingOrWillChangePlaymode;
    }

    [MenuItem("bip_project/Aula 1/Escurecer Praca Central")]
    private static void EscurecerPracaCentral()
    {
        if (!PodeEditarCena())
            return;

        Scene scene = SceneManager.GetActiveScene();
        GameObject zona1 = EncontrarRaiz(scene, "Zona1_Vila");
        Transform praca = zona1 != null ? zona1.transform.Find("Praca_Circular") : null;
        if (praca == null)
        {
            Debug.LogError("Nao encontrei a Praca_Circular. Monte o greybox primeiro.");
            return;
        }

        Renderer renderer = praca.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError("A Praca_Circular nao possui Renderer.", praca);
            return;
        }

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Escurecer Praca Central");
        AplicarMaterialPraca(praca.gameObject);
        EditorSceneManager.MarkSceneDirty(scene);
        Undo.CollapseUndoOperations(undoGroup);
    }

    [MenuItem("bip_project/Aula 1/Escurecer Praca Central", true)]
    private static bool ValidarEscurecerPracaCentral()
    {
        return !EditorApplication.isPlayingOrWillChangePlaymode;
    }

    [MenuItem("bip_project/Aula 1/Configurar Bip")]
    private static void ConfigurarBip()
    {
        if (!PodeEditarCena())
            return;

        GameObject bip = Selection.activeGameObject;
        if (bip == null || !bip.scene.IsValid() || !bip.scene.isLoaded)
        {
            Debug.LogError("Selecione o modelo do Bip na Hierarchy em uma cena aberta.");
            return;
        }

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Configurar Bip");

        if (!GarantirTagELayerPlayer(out int playerLayer))
            return;

        AplicarConfiguracaoBip(bip, playerLayer);
        Selection.activeGameObject = bip;
        EditorUtility.SetDirty(bip);
        EditorSceneManager.MarkSceneDirty(bip.scene);
        Undo.CollapseUndoOperations(undoGroup);
    }

    [MenuItem("bip_project/Aula 1/Inserir Prefabs dos Personagens")]
    private static void InserirPrefabsDosPersonagens()
    {
        if (!PodeEditarCena())
            return;

        Scene scene = SceneManager.GetActiveScene();
        if (!scene.IsValid() || !scene.isLoaded)
        {
            Debug.LogError("Abra uma cena antes de inserir os personagens.");
            return;
        }

        GameObject zona1 = GameObject.Find("Zona1_Vila");
        GameObject zona2 = GameObject.Find("Zona2_Patio");
        GameObject zona3 = GameObject.Find("Zona3_Forja");
        if (zona1 == null || zona2 == null || zona3 == null)
        {
            Debug.LogError("Monte primeiro o greybox pelo menu Montar Greybox.");
            return;
        }

        if (GameObject.Find("Bip") != null ||
            GameObject.Find("Arcano Lumen") != null ||
            GameObject.Find("Sentinela Rubra") != null ||
            GameObject.Find("Imperador Rubro") != null)
        {
            Debug.LogError("Um ou mais personagens ja estao na cena; remova-os antes de inserir novamente.");
            return;
        }

        GameObject bipPrefab = CarregarPrefabGlb("Bip.glb");
        GameObject arcanoPrefab = CarregarPrefabGlb("Arcano_Lumen.glb");
        GameObject sentinelaPrefab = CarregarPrefabGlb("Sentinela_Rubra.glb");
        GameObject imperadorPrefab = CarregarPrefabGlb("Imperador_Rubro.glb");
        if (bipPrefab == null || arcanoPrefab == null || sentinelaPrefab == null || imperadorPrefab == null)
            return;

        if (!GarantirTagELayerPlayer(out int playerLayer) || !GarantirLayer("Enemy", out int enemyLayer))
            return;

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Inserir Prefabs dos Personagens");

        GameObject bip = InstanciarPrefab(bipPrefab, scene, zona1.transform, "Bip",
            new Vector3(0f, 0f, -14f));
        GameObject arcano = InstanciarPrefab(arcanoPrefab, scene, zona1.transform, "Arcano Lumen",
            new Vector3(0f, 0.2f, 0f));
        GameObject sentinela = InstanciarPrefab(sentinelaPrefab, scene, zona2.transform, "Sentinela Rubra",
            Vector3.zero);
        GameObject imperador = InstanciarPrefab(imperadorPrefab, scene, zona3.transform, "Imperador Rubro",
            Vector3.zero);

        if (bip == null || arcano == null || sentinela == null || imperador == null)
        {
            Undo.RevertAllDownToGroup(undoGroup);
            return;
        }

        AplicarConfiguracaoBip(bip, playerLayer);
        AplicarLayerRecursiva(sentinela, enemyLayer);

        Selection.activeGameObject = bip;
        EditorUtility.SetDirty(bip);
        EditorSceneManager.MarkSceneDirty(scene);
        Undo.CollapseUndoOperations(undoGroup);
    }

    [MenuItem("bip_project/Aula 1/Inserir Prefabs dos Personagens", true)]
    private static bool ValidarInserirPrefabsDosPersonagens()
    {
        return !EditorApplication.isPlayingOrWillChangePlaymode;
    }

    private static GameObject CarregarPrefabGlb(string nomeArquivo)
    {
        string caminho = "Assets/_Project/Art/Characters/" + nomeArquivo;
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(caminho);
        if (prefab == null)
            Debug.LogError("Nao foi possivel carregar " + caminho + ". Aguarde a importacao do glTFast.");
        return prefab;
    }

    private static GameObject InstanciarPrefab(
        GameObject prefab,
        Scene scene,
        Transform pai,
        string nome,
        Vector3 posicaoLocal)
    {
        GameObject instancia = PrefabUtility.InstantiatePrefab(prefab, scene) as GameObject;
        if (instancia == null)
        {
            Debug.LogError("Nao foi possivel instanciar o prefab " + prefab.name + ".");
            return null;
        }

        Undo.RegisterCreatedObjectUndo(instancia, "Inserir " + nome);
        Undo.SetTransformParent(instancia.transform, pai, "Agrupar " + nome);
        Undo.RecordObject(instancia.transform, "Posicionar " + nome);
        instancia.name = nome;
        instancia.transform.localPosition = posicaoLocal;
        instancia.transform.localRotation = Quaternion.identity;
        return instancia;
    }

    private static void AplicarConfiguracaoBip(GameObject bip, int playerLayer)
    {
        Undo.RecordObject(bip, "Configurar Bip");
        bip.tag = "Player";
        AplicarLayerRecursiva(bip, playerLayer);

        CharacterController characterController = bip.GetComponent<CharacterController>();
        if (characterController == null)
            characterController = Undo.AddComponent<CharacterController>(bip);
        Undo.RecordObject(characterController, "Ajustar CharacterController do Bip");
        AjustarCharacterController(bip, characterController);

        PlayerMovement movement = bip.GetComponent<PlayerMovement>();
        if (movement == null)
            movement = Undo.AddComponent<PlayerMovement>(bip);
        Undo.RecordObject(movement, "Configurar movimento do Bip");
        movement.speed = 5f;
        movement.jumpHeight = 1.2f;
        movement.gravity = -20f;
        if (movement.cam == null)
            movement.cam = Camera.main;
        if (movement.animator == null)
            movement.animator = bip.GetComponentInChildren<Animator>();
    }

    [MenuItem("bip_project/Aula 1/Configurar Bip", true)]
    private static bool ValidarConfigurarBip()
    {
        GameObject bip = Selection.activeGameObject;
        return !EditorApplication.isPlayingOrWillChangePlaymode &&
               bip != null && bip.scene.IsValid() && bip.scene.isLoaded;
    }

    [MenuItem("bip_project/Aula 1/Configurar Camera do Bip")]
    private static void ConfigurarCameraDoBip()
    {
        if (!PodeEditarCena())
            return;

        GameObject bip = Selection.activeGameObject;
        if (bip == null || !bip.scene.IsValid() || !bip.scene.isLoaded)
        {
            Debug.LogError("Selecione o objeto raiz do Bip na Hierarchy.");
            return;
        }

        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Nao foi encontrada uma camera com a tag MainCamera.");
            return;
        }

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Configurar Camera do Bip");

        PlayerCameraController cameraController = mainCamera.GetComponent<PlayerCameraController>();
        if (cameraController == null)
            cameraController = Undo.AddComponent<PlayerCameraController>(mainCamera.gameObject);

        Undo.RecordObject(cameraController, "Seguir Bip com a camera");
        cameraController.target = bip.transform;

        PlayerMovement movement = bip.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            Undo.RecordObject(movement, "Usar camera principal no movimento do Bip");
            movement.cam = mainCamera;
            EditorUtility.SetDirty(movement);
        }

        EditorUtility.SetDirty(cameraController);
        EditorSceneManager.MarkSceneDirty(bip.scene);
        Undo.CollapseUndoOperations(undoGroup);
    }

    [MenuItem("bip_project/Aula 1/Configurar Camera do Bip", true)]
    private static bool ValidarConfigurarCameraDoBip()
    {
        GameObject bip = Selection.activeGameObject;
        return !EditorApplication.isPlayingOrWillChangePlaymode &&
               bip != null && bip.scene.IsValid() && bip.scene.isLoaded;
    }

    private static bool GarantirTagELayerPlayer(out int playerLayer)
    {
        return GarantirTagELayer("Player", true, out playerLayer);
    }

    private static bool GarantirLayer(string nome, out int layer)
    {
        return GarantirTagELayer(nome, false, out layer);
    }

    private static bool GarantirTagELayer(string nome, bool garantirTag, out int layer)
    {
        layer = -1;
        UnityEngine.Object[] settings = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if (settings.Length == 0)
        {
            Debug.LogError("Nao foi possivel carregar as configuracoes de tags e layers.");
            return false;
        }

        SerializedObject tagManager = new SerializedObject(settings[0]);
        tagManager.Update();
        SerializedProperty tags = tagManager.FindProperty("tags");
        SerializedProperty layers = tagManager.FindProperty("layers");
        if (tags == null || layers == null)
        {
            Debug.LogError("As propriedades de tags ou layers nao foram encontradas.");
            return false;
        }

        bool hasTag = !garantirTag || nome == "Player";
        if (garantirTag && !hasTag)
        {
            for (int i = 0; i < tags.arraySize; i++)
            {
                if (tags.GetArrayElementAtIndex(i).stringValue == nome)
                {
                    hasTag = true;
                    break;
                }
            }
        }

        for (int i = 0; i < layers.arraySize; i++)
        {
            if (layers.GetArrayElementAtIndex(i).stringValue == nome)
            {
                layer = i;
                break;
            }
        }

        bool hasLayer = layer >= 0;
        if (!hasLayer)
        {
            for (int i = 8; i < layers.arraySize; i++)
            {
                if (string.IsNullOrEmpty(layers.GetArrayElementAtIndex(i).stringValue))
                {
                    layer = i;
                    break;
                }
            }

            if (layer < 0)
            {
                Debug.LogError("Nao ha um slot livre para criar a layer " + nome + ".");
                return false;
            }
        }

        if (!hasTag || !hasLayer)
        {
            Undo.RecordObject(settings[0], "Criar tag/layer " + nome);
            if (!hasTag)
            {
                int tagIndex = tags.arraySize;
                tags.InsertArrayElementAtIndex(tagIndex);
                tags.GetArrayElementAtIndex(tagIndex).stringValue = nome;
            }

            if (!hasLayer)
                layers.GetArrayElementAtIndex(layer).stringValue = nome;

            tagManager.ApplyModifiedProperties();
            EditorUtility.SetDirty(settings[0]);
        }

        return true;
    }

    private static bool PodeEditarCena()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError("Saia do Play Mode antes de executar este menu.");
            return false;
        }

        return true;
    }

    private static void AplicarLayerRecursiva(GameObject objeto, int layer)
    {
        Undo.RecordObject(objeto, "Aplicar layer");
        objeto.layer = layer;
        foreach (Transform child in objeto.transform)
            AplicarLayerRecursiva(child.gameObject, layer);
    }

    private static void AjustarCharacterController(GameObject bip, CharacterController controller)
    {
        Renderer[] renderers = bip.GetComponentsInChildren<Renderer>(true);
        Vector3 min = Vector3.positiveInfinity;
        Vector3 max = Vector3.negativeInfinity;
        bool hasBounds = false;

        foreach (Renderer renderer in renderers)
        {
            Bounds bounds = renderer.bounds;
            for (int corner = 0; corner < 8; corner++)
            {
                Vector3 worldPoint = new Vector3(
                    (corner & 1) == 0 ? bounds.min.x : bounds.max.x,
                    (corner & 2) == 0 ? bounds.min.y : bounds.max.y,
                    (corner & 4) == 0 ? bounds.min.z : bounds.max.z);
                Vector3 localPoint = bip.transform.InverseTransformPoint(worldPoint);
                min = Vector3.Min(min, localPoint);
                max = Vector3.Max(max, localPoint);
                hasBounds = true;
            }
        }

        Vector3 size;
        if (hasBounds)
        {
            size = max - min;
            controller.center = (min + max) * 0.5f;
        }
        else
        {
            Debug.LogWarning("O modelo nao possui Renderer; usando dimensoes padrao para o CharacterController.", bip);
            size = new Vector3(0.7f, 1.8f, 0.7f);
            controller.center = new Vector3(0f, 0.9f, 0f);
        }

        float radius = Mathf.Max(0.1f, Mathf.Min(size.x, size.z) * 0.3f);
        controller.radius = radius;
        controller.height = Mathf.Max(size.y, radius * 2f);
        controller.skinWidth = Mathf.Min(0.08f, radius * 0.2f);
        controller.stepOffset = Mathf.Min(0.3f, controller.height * 0.25f);
        controller.slopeLimit = 45f;
    }

    [MenuItem("bip_project/Aula 1/Criar Animator do Bip")]
    private static void CriarAnimatorDoBip()
    {
        if (!PodeEditarCena())
            return;

        GameObject bip = Selection.activeGameObject;
        if (bip == null || !bip.scene.IsValid() || !bip.scene.isLoaded)
        {
            Debug.LogError("Selecione o Bip na Hierarchy em uma cena aberta.");
            return;
        }

        const string controllerPath = "Assets/_Project/Art/Characters/Bip.controller";
        if (!AssetDatabase.IsValidFolder("Assets/_Project/Art/Characters"))
        {
            Debug.LogError("A pasta Assets/_Project/Art/Characters nao existe.");
            return;
        }

        AnimationClip[] clips = CarregarClipesDoBip();
        AnimationClip idle = EncontrarClipe(clips, "idle", "parad", "descans", "respir", "ocioso", "stand");
        AnimationClip walk = EncontrarClipe(clips, "walk", "andar", "caminh", "passo");
        AnimationClip run = EncontrarClipe(clips, "run", "corr", "sprint");
        AvisarClipeAusente(idle, "Idle", bip);
        AvisarClipeAusente(walk, "Andar", bip);
        AvisarClipeAusente(run, "Correr", bip);

        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
        if (controller == null)
        {
            if (AssetDatabase.LoadMainAssetAtPath(controllerPath) != null)
            {
                Debug.LogError("Ja existe um asset em Bip.controller, mas ele nao e um Animator Controller.");
                return;
            }

            controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        }

        if (controller == null)
        {
            Debug.LogError("Nao foi possivel criar ou carregar Bip.controller.");
            return;
        }

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Criar Animator do Bip");
        Undo.RecordObject(controller, "Configurar Animator Controller do Bip");

        if (!GarantirParametroSpeed(controller))
            return;

        AnimatorControllerLayer[] layers = controller.layers;
        if (layers.Length == 0)
        {
            Debug.LogError("Bip.controller nao possui uma layer de Animator.");
            return;
        }

        AnimatorStateMachine stateMachine = layers[0].stateMachine;
        AnimatorState state = EncontrarEstado(stateMachine, "Locomocao");
        if (state == null)
            state = stateMachine.AddState("Locomocao");

        BlendTree blendTree = state.motion as BlendTree;
        if (blendTree == null)
        {
            blendTree = new BlendTree();
            blendTree.name = "Locomocao";
            AssetDatabase.AddObjectToAsset(blendTree, controller);
            state.motion = blendTree;
        }

        blendTree.blendType = BlendTreeType.Simple1D;
        blendTree.blendParameter = "Speed";
        blendTree.useAutomaticThresholds = false;
        blendTree.children = new[]
        {
            new ChildMotion { motion = idle, threshold = 0f, timeScale = 1f },
            new ChildMotion { motion = walk, threshold = 0.5f, timeScale = 1f },
            new ChildMotion { motion = run, threshold = 1f, timeScale = 1f }
        };
        stateMachine.defaultState = state;

        Animator animator = bip.GetComponent<Animator>();
        if (animator == null)
            animator = bip.GetComponentInChildren<Animator>();
        if (animator == null)
            animator = Undo.AddComponent<Animator>(bip);

        Undo.RecordObject(animator, "Atribuir Animator Controller ao Bip");
        animator.runtimeAnimatorController = controller;

        PlayerMovement movement = bip.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            Undo.RecordObject(movement, "Ligar Animator ao PlayerMovement");
            movement.animator = animator;
            EditorUtility.SetDirty(movement);
        }
        else
        {
            Debug.LogWarning("PlayerMovement nao foi encontrado no Bip; rode Configurar Bip primeiro.", bip);
        }

        EditorUtility.SetDirty(controller);
        EditorUtility.SetDirty(stateMachine);
        EditorUtility.SetDirty(blendTree);
        EditorUtility.SetDirty(bip);
        EditorSceneManager.MarkSceneDirty(bip.scene);
        AssetDatabase.SaveAssets();
        Undo.CollapseUndoOperations(undoGroup);
    }

    [MenuItem("bip_project/Aula 1/Criar Animator do Bip", true)]
    private static bool ValidarCriarAnimatorDoBip()
    {
        GameObject bip = Selection.activeGameObject;
        return !EditorApplication.isPlayingOrWillChangePlaymode &&
               bip != null && bip.scene.IsValid() && bip.scene.isLoaded;
    }

    private static AnimationClip[] CarregarClipesDoBip()
    {
        string[] caminhos =
        {
            "Assets/_Project/Art/Characters/Bip.fbx",
            "Assets/_Project/Art/Characters/Bip.glb"
        };
        List<AnimationClip> clips = new List<AnimationClip>();

        foreach (string caminho in caminhos)
        {
            if (AssetDatabase.LoadMainAssetAtPath(caminho) == null)
                continue;

            foreach (UnityEngine.Object asset in AssetDatabase.LoadAllAssetsAtPath(caminho))
            {
                AnimationClip clip = asset as AnimationClip;
                if (clip != null && !clip.name.StartsWith("__preview__", StringComparison.OrdinalIgnoreCase))
                    clips.Add(clip);
            }
        }

        return clips.ToArray();
    }

    private static AnimationClip EncontrarClipe(AnimationClip[] clips, params string[] termos)
    {
        foreach (string termo in termos)
        {
            foreach (AnimationClip clip in clips)
            {
                if (clip.name.IndexOf(termo, StringComparison.OrdinalIgnoreCase) >= 0)
                    return clip;
            }
        }

        return null;
    }

    private static void AvisarClipeAusente(AnimationClip clip, string nome, GameObject bip)
    {
        if (clip == null)
            Debug.LogWarning("Clipe " + nome + " nao encontrado no FBX/GLB do Bip; a entrada do Blend Tree ficara vazia.", bip);
    }

    private static bool GarantirParametroSpeed(AnimatorController controller)
    {
        foreach (AnimatorControllerParameter parameter in controller.parameters)
        {
            if (parameter.name != "Speed")
                continue;

            if (parameter.type != AnimatorControllerParameterType.Float)
            {
                Debug.LogError("O parametro Speed ja existe, mas nao e do tipo Float.");
                return false;
            }

            return true;
        }

        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
        return true;
    }

    private static AnimatorState EncontrarEstado(AnimatorStateMachine stateMachine, string nome)
    {
        foreach (ChildAnimatorState child in stateMachine.states)
        {
            if (child.state.name == nome)
                return child.state;
        }

        return null;
    }

    private static GameObject CriarGrupo(string nome, Vector3 posicao)
    {
        GameObject grupo = new GameObject(nome);
        Undo.RegisterCreatedObjectUndo(grupo, "Criar " + nome);
        grupo.transform.position = posicao;
        return grupo;
    }

    private static GameObject CriarPrimitiva(
        Transform pai,
        PrimitiveType tipo,
        string nome,
        Vector3 posicaoLocal,
        Vector3 escala)
    {
        GameObject objeto = GameObject.CreatePrimitive(tipo);
        Undo.RegisterCreatedObjectUndo(objeto, "Criar " + nome);
        objeto.name = nome;
        objeto.transform.SetParent(pai, false);
        objeto.transform.localPosition = posicaoLocal;
        objeto.transform.localScale = escala;

        AplicarCor(objeto, Gray);
        return objeto;
    }

    private static void AplicarCor(GameObject objeto, Color cor)
    {
        Renderer renderer = objeto.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError("Nao foi encontrado Renderer em " + objeto.name + ".", objeto);
            return;
        }

        MaterialPropertyBlock propriedades = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(propriedades);
        propriedades.SetColor("_BaseColor", cor);
        propriedades.SetColor("_Color", cor);
        renderer.SetPropertyBlock(propriedades);
    }

    private static void AplicarMaterialPraca(GameObject praca)
    {
        Renderer renderer = praca.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError("A Praca_Circular nao possui Renderer.", praca);
            return;
        }

        if (!AssetDatabase.IsValidFolder("Assets/_Project/Art"))
            AssetDatabase.CreateFolder("Assets/_Project", "Art");
        if (!AssetDatabase.IsValidFolder("Assets/_Project/Art/Materials"))
            AssetDatabase.CreateFolder("Assets/_Project/Art", "Materials");

        Material material = AssetDatabase.LoadAssetAtPath<Material>(PlazaMaterialPath);
        if (material == null)
        {
            if (AssetDatabase.LoadMainAssetAtPath(PlazaMaterialPath) != null)
            {
                Debug.LogError("O asset Plaza_DarkGray.mat existe, mas nao e um Material.");
                return;
            }

            if (renderer.sharedMaterial == null)
            {
                Debug.LogError("A Praca_Circular nao possui material para servir de base.", praca);
                return;
            }

            material = new Material(renderer.sharedMaterial);
            material.name = "Plaza_DarkGray";
            if (!DefinirCorMaterialPraca(material))
            {
                UnityEngine.Object.DestroyImmediate(material);
                return;
            }

            AssetDatabase.CreateAsset(material, PlazaMaterialPath);
        }
        else
        {
            Undo.RecordObject(material, "Atualizar material escuro da Praca");
            if (!DefinirCorMaterialPraca(material))
                return;
            EditorUtility.SetDirty(material);
        }

        Undo.RecordObject(renderer, "Atribuir material escuro a Praca");
        renderer.SetPropertyBlock(null);
        renderer.sharedMaterial = material;
        EditorUtility.SetDirty(renderer);
        AssetDatabase.SaveAssets();
    }

    private static bool DefinirCorMaterialPraca(Material material)
    {
        bool temCor = false;
        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", PlazaDarkGray);
            temCor = true;
        }
        if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", PlazaDarkGray);
            temCor = true;
        }

        if (!temCor)
            Debug.LogError("O shader do material da Praca_Circular nao possui propriedade de cor.");
        return temCor;
    }

    private static GameObject EncontrarRaiz(Scene scene, string nome)
    {
        foreach (GameObject raiz in scene.GetRootGameObjects())
        {
            if (raiz.name == nome)
                return raiz;
        }

        return null;
    }

    private static void AjustarPlataforma(Transform plataforma, Vector3 posicao, Vector3 escala)
    {
        Undo.RecordObject(plataforma, "Ajustar acesso da plataforma");
        plataforma.localPosition = posicao;
        plataforma.localScale = escala;
    }

    private static void ConfigurarColisorDaPlataforma(GameObject plataforma)
    {
        CapsuleCollider colisorCapsula = plataforma.GetComponent<CapsuleCollider>();
        if (colisorCapsula != null)
            Undo.DestroyObjectImmediate(colisorCapsula);

        BoxCollider colisorCaixa = plataforma.GetComponent<BoxCollider>();
        if (colisorCaixa == null)
            colisorCaixa = Undo.AddComponent<BoxCollider>(plataforma);

        Undo.RecordObject(colisorCaixa, "Ajustar colisor plano da plataforma");
        colisorCaixa.center = Vector3.zero;
        colisorCaixa.size = new Vector3(1f, 2f, 1f);
        colisorCaixa.isTrigger = false;
    }

    private static void CriarLimitesInvisiveis(Scene scene, Transform forja)
    {
        GameObject grupo = EncontrarRaiz(scene, "Limites_Invisiveis");
        if (grupo == null)
        {
            grupo = CriarGrupo("Limites_Invisiveis", Vector3.zero);
        }
        else
        {
            Undo.RecordObject(grupo.transform, "Posicionar limites invisiveis");
            grupo.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            grupo.transform.localScale = Vector3.one;
        }

        Transform limites = grupo.transform;
        const float alturaMuro = 8f;
        const float centroY = 3f;
        const float espessura = 1f;

        GarantirParedeInvisivel(limites, "Vila_Limite_Oeste",
            new Vector3(-20.5f, centroY, 0f), new Vector3(espessura, alturaMuro, 40f), Quaternion.identity);
        GarantirParedeInvisivel(limites, "Vila_Limite_Leste",
            new Vector3(20.5f, centroY, 0f), new Vector3(espessura, alturaMuro, 40f), Quaternion.identity);
        GarantirParedeInvisivel(limites, "Vila_Limite_Sul",
            new Vector3(0f, centroY, -20.5f), new Vector3(40f, alturaMuro, espessura), Quaternion.identity);
        GarantirParedeInvisivel(limites, "Vila_Limite_Norte_Esquerdo",
            new Vector3(-11.5f, centroY, 20.5f), new Vector3(17f, alturaMuro, espessura), Quaternion.identity);
        GarantirParedeInvisivel(limites, "Vila_Limite_Norte_Direito",
            new Vector3(11.5f, centroY, 20.5f), new Vector3(17f, alturaMuro, espessura), Quaternion.identity);
        GarantirParedeInvisivel(limites, "Corredor_Limite_Esquerdo",
            new Vector3(-3.5f, centroY, 30f), new Vector3(espessura, alturaMuro, 20f), Quaternion.identity);
        GarantirParedeInvisivel(limites, "Corredor_Limite_Direito",
            new Vector3(3.5f, centroY, 30f), new Vector3(espessura, alturaMuro, 20f), Quaternion.identity);

        const int segmentosArena = 24;
        const float raioArena = 15.25f;
        const float alturaMuroArena = 4f;
        const float centroYMuroArena = 1.3f;
        float passoAngulo = 360f / segmentosArena;
        float comprimentoSegmento = 2f * raioArena * Mathf.Sin(passoAngulo * 0.5f * Mathf.Deg2Rad) + 0.2f;
        for (int i = 0; i < segmentosArena; i++)
        {
            if (i == 12 || i == 13)
                continue;

            float angulo = (i - 0.5f) * passoAngulo;
            float radianos = angulo * Mathf.Deg2Rad;
            Vector3 localForja = new Vector3(
                Mathf.Sin(radianos) * raioArena,
                centroYMuroArena,
                Mathf.Cos(radianos) * raioArena);
            Vector3 posicaoMundo = forja.TransformPoint(localForja);
            Quaternion rotacaoMundo = forja.rotation * Quaternion.Euler(0f, angulo, 0f);
            Vector3 posicaoLocal = limites.InverseTransformPoint(posicaoMundo);
            Quaternion rotacaoLocal = Quaternion.Inverse(limites.rotation) * rotacaoMundo;
            GarantirParedeInvisivel(limites, "Forja_Limite_" + i.ToString("00"),
                posicaoLocal, new Vector3(comprimentoSegmento, alturaMuroArena, 0.6f), rotacaoLocal);
        }
    }

    private static void GarantirParedeInvisivel(
        Transform pai,
        string nome,
        Vector3 posicaoLocal,
        Vector3 tamanho,
        Quaternion rotacaoLocal)
    {
        Transform existente = pai.Find(nome);
        GameObject parede;
        if (existente == null)
        {
            parede = new GameObject(nome);
            Undo.RegisterCreatedObjectUndo(parede, "Criar limite invisivel " + nome);
            Undo.SetTransformParent(parede.transform, pai, "Agrupar limite invisivel");
        }
        else
        {
            parede = existente.gameObject;
            Undo.RecordObject(parede.transform, "Ajustar limite invisivel");
        }

        parede.transform.localPosition = posicaoLocal;
        parede.transform.localRotation = rotacaoLocal;
        parede.transform.localScale = Vector3.one;

        BoxCollider colisor = parede.GetComponent<BoxCollider>();
        if (colisor == null)
            colisor = Undo.AddComponent<BoxCollider>(parede);
        Undo.RecordObject(colisor, "Ajustar colisor invisivel");
        colisor.center = Vector3.zero;
        colisor.size = tamanho;
        colisor.isTrigger = false;
    }

    private static void CriarMurosDoPatio(Transform patio)
    {
        const float altura = 3f;
        const float espessura = 1f;
        const float larguraPortao = 6f;
        const float meiaLargura = 25f;
        const float meiaProfundidade = 15f;

        CriarPrimitiva(patio, PrimitiveType.Cube, "Muro_Lateral_Esquerdo",
            new Vector3(-meiaLargura + espessura * 0.5f, altura * 0.5f, 0f),
            new Vector3(espessura, altura, 30f));
        CriarPrimitiva(patio, PrimitiveType.Cube, "Muro_Lateral_Direito",
            new Vector3(meiaLargura - espessura * 0.5f, altura * 0.5f, 0f),
            new Vector3(espessura, altura, 30f));

        float segmentoLargura = (50f - larguraPortao) * 0.5f;
        float segmentoX = (larguraPortao + segmentoLargura) * 0.5f;
        CriarPrimitiva(patio, PrimitiveType.Cube, "Muro_Entrada_Esquerdo",
            new Vector3(-segmentoX, altura * 0.5f, -meiaProfundidade),
            new Vector3(segmentoLargura, altura, espessura));
        CriarPrimitiva(patio, PrimitiveType.Cube, "Muro_Entrada_Direito",
            new Vector3(segmentoX, altura * 0.5f, -meiaProfundidade),
            new Vector3(segmentoLargura, altura, espessura));
        CriarPrimitiva(patio, PrimitiveType.Cube, "Muro_Saida_Esquerdo",
            new Vector3(-segmentoX, altura * 0.5f, meiaProfundidade),
            new Vector3(segmentoLargura, altura, espessura));
        CriarPrimitiva(patio, PrimitiveType.Cube, "Muro_Saida_Direito",
            new Vector3(segmentoX, altura * 0.5f, meiaProfundidade),
            new Vector3(segmentoLargura, altura, espessura));
    }

    private static void CriarCaixas(Transform patio)
    {
        for (int i = 0; i < 10; i++)
        {
            int coluna = i % 5;
            int linha = i / 5;
            float x = -16f + coluna * 8f;
            float z = -6f + linha * 12f;
            CriarPrimitiva(patio, PrimitiveType.Cube, "Caixa_" + (i + 1).ToString("00"),
                new Vector3(x, 0.75f, z), Vector3.one * 1.5f);
        }
    }

    private static void CriarPilares(Transform forja)
    {
        Vector3[] posicoes =
        {
            new Vector3(-9f, 2.5f, -9f),
            new Vector3(9f, 2.5f, -9f),
            new Vector3(-9f, 2.5f, 9f),
            new Vector3(9f, 2.5f, 9f)
        };

        for (int i = 0; i < posicoes.Length; i++)
        {
            CriarPrimitiva(forja, PrimitiveType.Cube, "Pilar_" + (i + 1).ToString("00"),
                posicoes[i], new Vector3(2f, 5f, 2f));
        }
    }

    private static void AjustarLuzDirecional()
    {
        Light[] luzes = UnityEngine.Object.FindObjectsByType<Light>();
        Light direcional = null;
        foreach (Light luz in luzes)
        {
            if (luz.type == LightType.Directional)
            {
                direcional = luz;
                break;
            }
        }

        if (direcional == null)
        {
            GameObject objeto = new GameObject("Directional Light");
            Undo.RegisterCreatedObjectUndo(objeto, "Criar Directional Light");
            direcional = objeto.AddComponent<Light>();
            direcional.type = LightType.Directional;
        }
        else
        {
            Undo.RecordObject(direcional, "Ajustar Directional Light");
        }

        direcional.color = new Color(0.65f, 0.78f, 1f);
    }

    private static void CriarLuzVermelha(Transform forja)
    {
        GameObject objeto = new GameObject("Luz_Vermelha_Forja");
        Undo.RegisterCreatedObjectUndo(objeto, "Criar Luz Vermelha da Forja");
        objeto.transform.SetParent(forja, false);
        objeto.transform.localPosition = new Vector3(0f, 3f, 0f);

        Light luz = objeto.AddComponent<Light>();
        luz.type = LightType.Point;
        luz.color = Color.red;
        luz.intensity = 5f;
        luz.range = 20f;
    }
}
