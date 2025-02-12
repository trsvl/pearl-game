using Gameplay.BallThrowing;
using Gameplay.Header;
using TMPro;
using UnityEngine;
using Utils.PlayerData;
using Utils.SphereData;

namespace Gameplay
{
    public class GameplayInstaller : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI pearlsText;
        [SerializeField] private TextMeshProUGUI shotsText;
        [SerializeField] private BallThrower ballThrower;
        [SerializeField] private SphereDestroyer sphereDestroyer;

        private SphereGenerator sphereGenerator;
        private DataContext dataContext;
        private PearlsData pearlsData;
        private ShotsData shotsData;


        private void Awake()
        {
            CreateSpheres();

            pearlsData = new PearlsData(pearlsText);

            int shotsCount = sphereGenerator._materials.Length * 2;
            shotsData = new ShotsData(shotsText, shotsCount);

            ballThrower.Init(sphereGenerator._materials, shotsData);
            sphereDestroyer.Init(pearlsData);
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