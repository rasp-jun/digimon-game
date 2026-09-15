using UnityEngine;

// Procedural low-poly prototypes for the six current baby-stage roster units.
// All parts are real 3D meshes, not camera-facing sprites.
public static class Digimon3DModelFactory
{
    public static void Build(string id, Transform root, out Transform leftLimb, out Transform rightLimb, out Transform tail)
    {
        leftLimb = rightLimb = tail = null;
        GameObject authored=Resources.Load<GameObject>("Models/"+id);
        if(authored!=null)
        {
            GameObject model=Object.Instantiate(authored,root,false);model.name=id+" Authored Model";
            model.transform.localPosition=Vector3.zero;model.transform.localRotation=Quaternion.Euler(0,180,0);model.transform.localScale=Vector3.one*.72f;
            leftLimb=Find(model.transform,"Ear.L");rightLimb=Find(model.transform,"Ear.R");tail=Find(model.transform,"Tail");return;
        }
        switch (id)
        {
            case "koromon": BuildKoromon(root, out leftLimb, out rightLimb); break;
            case "tsunomon": BuildTsunomon(root, out leftLimb, out rightLimb, out tail); break;
            case "mochimon": BuildMochimon(root, out leftLimb, out rightLimb); break;
            case "tanemon": BuildTanemon(root, out leftLimb, out rightLimb, out tail); break;
            case "pyocomon": BuildPyocomon(root, out leftLimb, out rightLimb, out tail); break;
            case "tokomon": BuildTokomon(root, out leftLimb, out rightLimb); break;
            default: BuildFallback(root, out leftLimb, out rightLimb); break;
        }
    }

    static Transform Find(Transform root,string name)
    { foreach(Transform child in root.GetComponentsInChildren<Transform>(true))if(child.name==name)return child;return null; }

    static Material Mat(Color color)
    {
        Shader shader = Resources.Load<Shader>("Digimon3D");
        Material result = new Material(shader); result.color = color; return result;
    }

    static Transform Part(Transform parent, string name, PrimitiveType type, Vector3 position, Vector3 scale, Color color, Vector3 rotation = default)
    {
        GameObject item = GameObject.CreatePrimitive(type); item.name = name; item.transform.SetParent(parent, false);
        item.transform.localPosition = position; item.transform.localScale = scale; item.transform.localEulerAngles = rotation;
        item.GetComponent<Renderer>().material = Mat(color); Object.Destroy(item.GetComponent<Collider>()); return item.transform;
    }

    static void Eyes(Transform root, float y, float z, float spread, float size, Color iris)
    {
        for (int side = -1; side <= 1; side += 2)
        {
            Transform white = Part(root, "Eye", PrimitiveType.Sphere, new Vector3(spread * side, y, z), new Vector3(size, size * 1.25f, size * .45f), Color.white);
            Part(white, "Iris", PrimitiveType.Sphere, new Vector3(0, 0, .78f), new Vector3(.45f, .55f, .3f), iris);
        }
    }

    static void Mouth(Transform root, float y, float z, float width)
    { Part(root, "Mouth", PrimitiveType.Cube, new Vector3(0, y, z), new Vector3(width, .035f, .025f), new Color(.24f, .035f, .06f), new Vector3(0, 0, -4)); }

    static void BuildKoromon(Transform r, out Transform l, out Transform rr)
    {
        Color pink = new Color(1f,.38f,.45f); Part(r,"Round Body",PrimitiveType.Sphere,new Vector3(0,.58f,0),new Vector3(1.12f,.88f,.92f),pink);
        l=Part(r,"Left Ear",PrimitiveType.Sphere,new Vector3(-.53f,.92f,0),new Vector3(.34f,.62f,.28f),pink,new Vector3(0,0,-38));
        rr=Part(r,"Right Ear",PrimitiveType.Sphere,new Vector3(.53f,.92f,0),new Vector3(.34f,.62f,.28f),pink,new Vector3(0,0,38));
        Eyes(r,.7f,.43f,.23f,.18f,new Color(.25f,.18f,.16f)); Mouth(r,.43f,.47f,.24f);
    }

    static void BuildTsunomon(Transform r, out Transform l, out Transform rr, out Transform tail)
    {
        Color fur=new Color(.82f,.76f,.64f); Part(r,"Furry Body",PrimitiveType.Sphere,new Vector3(0,.57f,0),new Vector3(1.05f,.9f,.92f),fur);
        Part(r,"Horn",PrimitiveType.Cylinder,new Vector3(0,1.22f,.02f),new Vector3(.18f,.48f,.18f),new Color(.35f,.16f,.10f),new Vector3(0,0,0));
        l=Part(r,"Left Paw",PrimitiveType.Sphere,new Vector3(-.48f,.37f,.12f),new Vector3(.35f,.28f,.4f),fur);
        rr=Part(r,"Right Paw",PrimitiveType.Sphere,new Vector3(.48f,.37f,.12f),new Vector3(.35f,.28f,.4f),fur);
        tail=Part(r,"Tail",PrimitiveType.Sphere,new Vector3(0,.42f,-.48f),new Vector3(.35f,.28f,.55f),fur,new Vector3(28,0,0));
        Eyes(r,.72f,.43f,.22f,.17f,new Color(.12f,.26f,.45f)); Mouth(r,.46f,.48f,.2f);
    }

    static void BuildMochimon(Transform r, out Transform l, out Transform rr)
    {
        Color green=new Color(.48f,.84f,.48f), shell=new Color(.72f,.92f,.62f); Part(r,"Soft Body",PrimitiveType.Sphere,new Vector3(0,.48f,0),new Vector3(1.14f,.68f,.92f),green);
        Part(r,"Shell",PrimitiveType.Sphere,new Vector3(0,.69f,-.08f),new Vector3(.95f,.72f,.72f),shell);
        l=Part(r,"Left Arm",PrimitiveType.Sphere,new Vector3(-.58f,.45f,.08f),new Vector3(.38f,.22f,.28f),green,new Vector3(0,0,-18));
        rr=Part(r,"Right Arm",PrimitiveType.Sphere,new Vector3(.58f,.45f,.08f),new Vector3(.38f,.22f,.28f),green,new Vector3(0,0,18));
        Eyes(r,.58f,.45f,.24f,.16f,new Color(.1f,.28f,.18f)); Mouth(r,.35f,.49f,.18f);
    }

    static void BuildTanemon(Transform r, out Transform l, out Transform rr, out Transform tail)
    {
        Color tan=new Color(.76f,.68f,.43f), leaf=new Color(.32f,.78f,.28f); Part(r,"Seed Body",PrimitiveType.Sphere,new Vector3(0,.56f,0),new Vector3(.9f,1.02f,.8f),tan);
        tail=Part(r,"Sprout Stem",PrimitiveType.Cylinder,new Vector3(0,1.16f,0),new Vector3(.1f,.28f,.1f),leaf);
        Part(tail,"Left Leaf",PrimitiveType.Sphere,new Vector3(-.24f,.42f,0),new Vector3(.48f,.12f,.25f),leaf,new Vector3(0,0,-25));
        Part(tail,"Right Leaf",PrimitiveType.Sphere,new Vector3(.24f,.42f,0),new Vector3(.48f,.12f,.25f),leaf,new Vector3(0,0,25));
        l=Part(r,"Left Foot",PrimitiveType.Sphere,new Vector3(-.3f,.1f,.06f),new Vector3(.38f,.2f,.42f),tan); rr=Part(r,"Right Foot",PrimitiveType.Sphere,new Vector3(.3f,.1f,.06f),new Vector3(.38f,.2f,.42f),tan);
        Eyes(r,.67f,.38f,.19f,.15f,new Color(.16f,.34f,.12f)); Mouth(r,.42f,.42f,.18f);
    }

    static void BuildPyocomon(Transform r, out Transform l, out Transform rr, out Transform tail)
    {
        Color rose=new Color(.93f,.28f,.48f), cream=new Color(1f,.78f,.62f); Part(r,"Bird Body",PrimitiveType.Sphere,new Vector3(0,.56f,0),new Vector3(.9f,.98f,.82f),rose);
        l=Part(r,"Left Wing",PrimitiveType.Sphere,new Vector3(-.48f,.58f,0),new Vector3(.23f,.62f,.38f),cream,new Vector3(0,0,-35)); rr=Part(r,"Right Wing",PrimitiveType.Sphere,new Vector3(.48f,.58f,0),new Vector3(.23f,.62f,.38f),cream,new Vector3(0,0,35));
        tail=Part(r,"Head Feather",PrimitiveType.Capsule,new Vector3(0,1.18f,-.03f),new Vector3(.2f,.38f,.2f),rose,new Vector3(0,0,-22));
        Part(r,"Beak",PrimitiveType.Sphere,new Vector3(0,.53f,.48f),new Vector3(.32f,.16f,.3f),new Color(1f,.72f,.18f)); Eyes(r,.75f,.38f,.2f,.14f,new Color(.32f,.08f,.12f));
    }

    static void BuildTokomon(Transform r, out Transform l, out Transform rr)
    {
        Color white=new Color(.92f,.92f,.84f); Part(r,"Oval Body",PrimitiveType.Sphere,new Vector3(0,.53f,0),new Vector3(1.08f,.82f,.9f),white);
        l=Part(r,"Left Ear",PrimitiveType.Capsule,new Vector3(-.4f,1.0f,0),new Vector3(.26f,.58f,.26f),white,new Vector3(0,0,-38)); rr=Part(r,"Right Ear",PrimitiveType.Capsule,new Vector3(.4f,1.0f,0),new Vector3(.26f,.58f,.26f),white,new Vector3(0,0,38));
        Eyes(r,.68f,.43f,.23f,.17f,new Color(.18f,.28f,.52f)); Mouth(r,.4f,.47f,.3f);
        for(int side=-1;side<=1;side+=2)Part(r,"Fang",PrimitiveType.Cylinder,new Vector3(.16f*side,.34f,.49f),new Vector3(.045f,.13f,.045f),Color.white);
    }

    static void BuildFallback(Transform r, out Transform l, out Transform rr)
    {
        Color blue=new Color(.28f,.62f,.82f); Part(r,"Body",PrimitiveType.Capsule,new Vector3(0,.72f,0),new Vector3(.7f,.75f,.7f),blue);
        l=Part(r,"Left Arm",PrimitiveType.Capsule,new Vector3(-.5f,.72f,0),new Vector3(.18f,.48f,.18f),blue,new Vector3(0,0,-35)); rr=Part(r,"Right Arm",PrimitiveType.Capsule,new Vector3(.5f,.72f,0),new Vector3(.18f,.48f,.18f),blue,new Vector3(0,0,35)); Eyes(r,.93f,.34f,.19f,.13f,Color.black);
    }
}
