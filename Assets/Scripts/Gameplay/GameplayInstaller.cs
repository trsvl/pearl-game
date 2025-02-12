using Gameplay.BallThrowing;
using UnityEngine;
using Utils.PlayerData;
using Utils.SphereData;

namespace Gameplay
{
    public class GameplayInstaller : MonoBehaviour
    {
        [SerializeField] private BallThrower ballThrower;

        private SphereGenerator sphereGenerator;
        private DataContext dataContext;


        private void Awake()
        {
            CreateSpheres();
            ballThrower.Init(sphereGenerator._materials);
        }

        private void CreateSpheres()
        {
            var spherePrefab = Resources.Load<GameObject>("Prefabs/Sphere");
            dataContext = new DataContext();

            sphereGenerator = new GameObject().AddComponent<SphereGenerator>();
            sphereGenerator.Init(spherePrefab);
            sphereGenerator.transform.position = new Vector3(0f, 3f, 0f);
            
            dataContext.UpdateFilePath(PlayerData.Instance.CurrentLevel);
            sphereGenerator.LoadSpheresFromJSON(dataContext.LoadSpheresJSON());
        }
    }
}