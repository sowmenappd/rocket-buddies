public interface ISpawnable{
    ISpawnable Spawn(UnityEngine.Vector3 pos);
    UnityEngine.GameObject GetGameObject();
    void Destroy();
}