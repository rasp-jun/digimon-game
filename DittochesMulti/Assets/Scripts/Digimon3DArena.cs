using System.Collections.Generic;
using UnityEngine;

public sealed class Digimon3DArena : MonoBehaviour
{
    public RenderTexture Texture { get; private set; }
    readonly List<Digimon3DActor> actors = new List<Digimon3DActor>();
    Transform actorRoot;

    public static Digimon3DArena Create(Transform owner)
    {
        GameObject root = new GameObject("3D Battle Arena");
        root.transform.SetParent(owner, false);
        Digimon3DArena arena = root.AddComponent<Digimon3DArena>();
        arena.Build(); return arena;
    }

    void Build()
    {
        const int battleLayer = 30;
        SetLayer(gameObject, battleLayer);
        actorRoot = new GameObject("Actors").transform; actorRoot.SetParent(transform, false); SetLayer(actorRoot.gameObject, battleLayer);
        Texture = new RenderTexture(1040, 670, 24, RenderTextureFormat.ARGB32) { name="Digimon 3D Battle" }; Texture.Create();

        GameObject cameraObject = new GameObject("Battle Camera"); cameraObject.transform.SetParent(transform, false); SetLayer(cameraObject, battleLayer);
        Camera camera = cameraObject.AddComponent<Camera>(); camera.targetTexture = Texture; camera.cullingMask = 1 << battleLayer;
        camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = new Color(0,0,0,0); camera.fieldOfView = 34;
        camera.transform.position = new Vector3(0, 10.8f, -11.5f); camera.transform.LookAt(new Vector3(0,0,0));
        AddLight("Key Light",new Vector3(-3,8,-5),new Color(.62f,.92f,1f),1.4f,battleLayer);
        AddLight("Rim Light",new Vector3(5,5,4),new Color(1f,.65f,.25f),1.0f,battleLayer);
    }

    public Digimon3DActor Spawn(string id, bool enemy, Vector2 boardPosition)
    {
        GameObject actorObject = new GameObject(id+" 3D"); actorObject.transform.SetParent(actorRoot,false); SetLayer(actorObject,30);
        actorObject.transform.localPosition = BoardToWorld(boardPosition); Digimon3DActor actor=actorObject.AddComponent<Digimon3DActor>(); actor.Initialize(id,enemy);
        SetLayer(actorObject,30); actors.Add(actor); return actor;
    }

    public void ClearActors()
    {
        foreach(Digimon3DActor actor in actors) if(actor) Destroy(actor.gameObject);
        actors.Clear();
    }

    public static Vector3 BoardToWorld(Vector2 board) { return new Vector3((board.x-3f)*1.08f,.2f,(board.y-3.5f)*.94f); }

    void AddLight(string name,Vector3 position,Color color,float intensity,int layer)
    { GameObject go=new GameObject(name); go.transform.SetParent(transform,false); go.transform.localPosition=position; Light light=go.AddComponent<Light>(); light.type=LightType.Point;light.range=18;light.color=color;light.intensity=intensity;SetLayer(go,layer); }
    static void SetLayer(GameObject root,int layer) { root.layer=layer; foreach(Transform child in root.transform)SetLayer(child.gameObject,layer); }

    void OnDestroy() { if(Texture!=null){Texture.Release();Destroy(Texture);} }
}
