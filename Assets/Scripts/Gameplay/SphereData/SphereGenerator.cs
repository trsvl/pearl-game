using UnityEngine;
using VContainer;

namespace Gameplay.SphereData
{
    public class SphereGenerator : MonoBehaviour, IStartGame, ILoseGame, IPauseGame, IResumeGame, IFinishGame
    {
        public Color[] _levelColors { get; private set; }
        public int allPearlsCount { get; private set; }

        protected AllColors _allColors { get; private set; }

        private GameObject _spherePrefab;
        private Quaternion _sphereRotation;
        private SpheresDictionary _spheresDictionary;
        private bool _isAllowedToRotate;


        [Inject]
        public void Init(GameObject spherePrefab, AllColors allColors, SpheresDictionary spheresDictionary)
        {
            _spherePrefab = spherePrefab;
            _allColors = allColors;
            _spheresDictionary = spheresDictionary;
            _sphereRotation = SphereRotation.GetQuaternion;

            transform.position = new Vector3(0f, 1f, 40f);
        }

        protected virtual void Update()
        {
            if (!_isAllowedToRotate) return;

            transform.rotation *= Quaternion.Euler(12f * Time.deltaTime, 12f * Time.deltaTime, 0);

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).rotation = _sphereRotation;
            }
        }

        public void LoadSpheres(SpheresData json)
        {
            ClearSpheres();

            _levelColors = new Color[json.colorNames.Length];

            for (int i = 0; i < json.colorNames.Length; i++)
            {
                Color color = _allColors.GetColor(json.colorNames[i]);
                _spheresDictionary.AddColorToDictionary(color, json.spheres.Length);
                _levelColors[i] = color;
            }
            
            GenerateBigSphereData(json);
        }

        protected virtual void GenerateBigSphereData(SpheresData data)
        {
            for (int i = 0; i < data.spheres.Length; i++)
            {
                var bigSphere = new BigSphere(data.spheres[i]);
                allPearlsCount += data.spheres[i].smallSphereCount;
                GenerateSmallSpheres(data, bigSphere, i);
            }
        }

        protected void GenerateSmallSpheres(SpheresData data, BigSphere bigSphere, int bigSphereIndex)
        {
            bigSphere.CreateSmallSpheres(_spherePrefab, transform, data.spheres[bigSphereIndex], _levelColors,
                _spheresDictionary, _sphereRotation, bigSphereIndex);
        }

        protected void ClearSpheres()
        {
            _spheresDictionary.DestroyAllSpheres();
        }

        public void StartGame()
        {
            _isAllowedToRotate = true;
        }

        public void LoseGame()
        {
            _isAllowedToRotate = false;
        }

        public void PauseGame()
        {
            _isAllowedToRotate = false;
        }

        public void ResumeGame()
        {
            _isAllowedToRotate = true;
        }

        public void FinishGame()
        {
            _isAllowedToRotate = false;
        }
    }
}