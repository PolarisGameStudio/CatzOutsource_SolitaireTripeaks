using Nightingale.Inputs;
using Nightingale.Utilitys;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Nightingale.ScenesManager
{
    public class MySceneManager : SingletonClass<MySceneManager>
    {
        private List<BaseScene> scenes = new List<BaseScene>();

        public T Navigation<T>(string name, NavigationEffect _navigation = null) where T : BaseScene
        {
            return Navigation<T>(Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(T).Name, name)), _navigation);
        }

        public T Navigation<T>(GameObject sceneObject, NavigationEffect _navigation = null) where T : BaseScene
        {
            if (_navigation == null)
            {
                _navigation = new NavigationEffect();
            }
            BaseScene[] array = (from e in scenes
                                 where !e.IsFixed
                                 select e).ToArray();
            BaseScene[] array2 = array;
            foreach (BaseScene scene in array2)
            {
                scene.SetSceneState(SceneState.Closing);
                _navigation.Closed(scene, delegate
                {
                    scene.SetSceneState(SceneState.Closed);
                });
                scenes.Remove(scene);
            }
            T val = ShowNewScene<T>(sceneObject, _navigation);
            scenes.Insert(0, val);
            BackgroundController.Get().SetActive(scenes.Count > 1);
            UpdateLayer();
            return val;
        }

        public T Popup<T>(string name, NavigationEffect effect = null) where T : BaseScene
        {
            return Popup<T>(Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(T).Name, name)), effect);
        }

        public T Popup<T>(GameObject sceneObject, NavigationEffect effect = null) where T : BaseScene
        {
            if (effect == null)
            {
                effect = new JoinEffect();
            }
            BaseScene LastTopScene = GetTopScene();
            if (Count() > 1 && !LastTopScene.IsStay)
            {
                LastTopScene.SetSceneState(SceneState.Hiding);
                effect.Hide(LastTopScene, delegate
                {
                    LastTopScene.SetSceneState(SceneState.Hided);
                });
            }
            else
            {
                LastTopScene.SetSceneState(SceneState.Hided);
            }
            BackgroundController.Get().SetActive(active: true);
            T val = ShowNewScene<T>(sceneObject, effect);
            scenes.Add(val);
            UpdateLayer();
            return val;
        }

        public int Count()
        {
            return scenes.Count;
        }

        public void Close(NavigationEffect effect = null, UnityAction unityAction = null)
        {
            if (Count() > 1)
            {
                if (effect == null)
                {
                    effect = new JoinEffect();
                }
                BaseScene LastTopScene = GetTopScene();
                scenes.RemoveAt(scenes.Count - 1);
                LastTopScene.SetSceneState(SceneState.Closing);
                SingletonBehaviour<EscapeInputManager>.Get().AppendKey("CloseScene");
                effect.Closed(LastTopScene, delegate
                {
                    LastTopScene.SetSceneState(SceneState.Closed);
                    BackgroundController.Get().SetActive(scenes.Count > 1);
                    SingletonBehaviour<EscapeInputManager>.Get().RemoveKey("CloseScene");
                    if (unityAction != null)
                    {
                        unityAction();
                    }
                });
                BaseScene NewTopScene = GetTopScene();
                if (Count() > 1 && !NewTopScene.IsStay)
                {
                    NewTopScene.SetSceneState(SceneState.Showing);
                    effect.Show(NewTopScene, delegate
                    {
                        NewTopScene.SetSceneState(SceneState.Showed);
                    });
                }
                else
                {
                    NewTopScene.SetSceneState(SceneState.Showed);
                }
                UpdateLayer();
            }
        }

        public void Close(BaseScene baseScene, NavigationEffect effect = null, UnityAction unityAction = null)
        {
            if (baseScene == null || !scenes.Contains(baseScene))
            {
                return;
            }
            if (scenes.IndexOf(baseScene) == scenes.Count - 1)
            {
                Close(effect);
                return;
            }
            scenes.Remove(baseScene);
            if (effect == null)
            {
                effect = new JoinEffect();
            }
            SingletonBehaviour<EscapeInputManager>.Get().AppendKey("CloseScene");
            baseScene.SetSceneState(SceneState.Closing);
            effect.Closed(baseScene, delegate
            {
                SingletonBehaviour<EscapeInputManager>.Get().RemoveKey("CloseScene");
                baseScene.SetSceneState(SceneState.Closed);
            });
        }

        public BaseScene GetDownScene()
        {
            if (scenes.Count == 0)
            {
                return null;
            }
            return scenes[0];
        }

        public BaseScene GetTopScene()
        {
            if (Count() == 0)
            {
                return null;
            }
            return scenes[scenes.Count - 1];
        }

        public List<BaseScene> GetScenes()
        {
            return scenes.ToList();
        }

        private unsafe T ShowNewScene<T>(GameObject asset, NavigationEffect effect) where T : BaseScene
        {
            asset.transform.SetAsLastSibling();
            T baseScene = (T)asset.GetComponent<T>();
            if ((Object)baseScene == (Object)null)
            {
                baseScene = (T)asset.AddComponent<T>();
            }
            if (effect != null)
            {
                //@TODO

                //((T*)(&baseScene))->SetSceneState(SceneState.Opening);
                //effect.Open(baseScene, delegate
                //{
                //	((T*)(&baseScene))->SetSceneState(SceneState.Opened);
                //});

                ((BaseScene)(baseScene)).SetSceneState(SceneState.Opening);
                effect.Open(baseScene, delegate
                {
                    ((BaseScene)(baseScene)).SetSceneState(SceneState.Opened);
                });
            }
            else
            {
                //((T*)(&baseScene))->SetSceneState(SceneState.Opened);
                ((BaseScene)(baseScene)).SetSceneState(SceneState.Opened);
            }
            return (T)baseScene;
        }

        private void UpdateLayer()
        {
            string[] array = (from e in SortingLayer.layers
                              select e.name).ToArray();
            if (array.Length < 2)
            {
                UnityEngine.Debug.LogError("需要至少2个SortingLayer...");
            }
            else
            {
                if (scenes.Count == 0)
                {
                    return;
                }
                if (scenes.Count == 1)
                {
                    BackgroundController.Get().SetAnimation(active: false);
                    scenes[0].SetLayer(array[1], 0);
                    return;
                }
                for (int i = 0; i < scenes.Count; i++)
                {
                    scenes[i].SetLayer(array[1], i * 2);
                }
                BackgroundController.Get().SetAnimation(active: true);
                BackgroundController.Get().SetLayer(array[1], scenes.Count * 2 - 3);
            }
        }
    }
}