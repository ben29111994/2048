using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using UnityEngine.EventSystems;
using System.Linq;
using VisCircle;
using UnityEngine.PostProcessing;
using System.Globalization;
using GameAnalyticsSDK;

public class GameController : MonoBehaviour
{
    [Header("Variable")]
    public static GameController instance;
    public bool isStartGame = false;
    public bool isControl = true;
    bool isDrag = false; 
    bool isVibrate = false;
    public float speed;
    public LayerMask dragMask;
    public List<int> listValueSpawn = new List<int>();
    public List<Color> listColor = new List<Color>();
    int biggestNum = 0;
    public int currentTask;
    public int score;
    public int moves;
    public static int bonus;
    int multitask = 0;
    Coroutine spawning;

    [Header("UI")]
    public Image progress;
    public GameObject winPanel;
    public GameObject losePanel;
    public Text currentLevelText;
    public Text nextLevelText;
    public int currentLevel;
    public Canvas canvas;
    public GameObject startGameMenu;
    public GameObject shopMenu;
    static int currentBG = 0;
    public InputField levelInput;
    public Text taskText;
    public Text coinText;
    int coin;
    public Text scoreText;
    public Text movesText;
    public Text scorePopup;
    public Text bonusPopup;
    public Text multitaskPopup;
    public GameObject hand;

    [Header("Objects")]
    public GameObject conffeti;
    public GameObject blast;
    public List<Color> envColor = new List<Color>();
    public List<GameObject> envMap = new List<GameObject>();
    public GameObject floor;
    public GameObject dieEffect;
    public List<GameObject> listChoose = new List<GameObject>();
    public List<GameObject> listNotChoose = new List<GameObject>();

    [System.Serializable]
    public struct valueCheck
    {
        public int num;
        public int count;
    }
    public List<valueCheck> listValueCheck = new List<valueCheck>();

    [System.Serializable]
    public struct task
    {
        public int num;
        public List<int> listValueInTask;
    }
    public List<task> listTasks = new List<task>();

    public GameObject ballPrefab;
    public Transform listPre;

    private void OnEnable()
    {
        // PlayerPrefs.DeleteAll();
        Application.targetFrameRate = 60;
        instance = this;
        StartCoroutine(delayStart());
    }

    IEnumerator delayStart()
    {
        startGameMenu.SetActive(true);
        Camera.main.transform.DOMoveX(-100, 0);
        Camera.main.transform.DOMoveX(0, 1);
        yield return new WaitForSeconds(0.001f);
        currentLevel = PlayerPrefs.GetInt("currentLevel");
        currentLevelText.text = currentLevel.ToString();
        //PlayerPrefs.SetInt("currentTask", 12);
        currentTask = PlayerPrefs.GetInt("currentTask");
        listValueSpawn = listTasks[currentTask].listValueInTask;
        var mission = listTasks[currentTask].num;
        string missionText;
        if (mission >= 4000)
        {
            mission /= 1000;
            missionText = mission.ToString() + "K";
        }
        else
        {
            missionText = mission.ToString();
        }
        taskText.text = "REACH " + missionText;
        countRandom = listValueSpawn.Count * 3;
        listValueCheck.Clear();
        foreach(var item in listValueSpawn)
        {
            valueCheck newItem = new valueCheck();
            newItem.num = item;
            listValueCheck.Add(newItem);
        }
        currentBG = Random.Range(0, envColor.Count);
        floor.GetComponent<Renderer>().material.color = envColor[currentBG];
        Camera.main.backgroundColor = envColor[currentBG] /** 1.05f*/;
        //RenderSettings.fogColor = envColor[currentBG] /** 1.05f*/;
        //foreach (var item in envMap)
        //{
        //    item.SetActive(false);
        //}
        //envMap[currentBG].SetActive(true);
        if (currentBG == 2)
        {
            currentLevelText.color = Color.white;
            currentLevelText.transform.parent.transform.GetChild(2).GetComponent<Text>().color = Color.white;
            taskText.color = Color.white;
            scoreText.transform.parent.transform.GetChild(0).GetComponent<Text>().color = Color.white;
            movesText.transform.parent.transform.GetChild(0).GetComponent<Text>().color = Color.white;
            scorePopup.color = Color.white;
            bonusPopup.color = Color.white;
            multitaskPopup.color = Color.white;
        }
        else
        {
            currentLevelText.color = Color.black;
            currentLevelText.transform.parent.transform.GetChild(2).GetComponent<Text>().color = Color.black;
            taskText.color = Color.black;
            scoreText.transform.parent.transform.GetChild(0).GetComponent<Text>().color = Color.black;
            movesText.transform.parent.transform.GetChild(0).GetComponent<Text>().color = Color.black;
            scorePopup.color = Color.black;
            bonusPopup.color = Color.black;
            multitaskPopup.color = Color.black;
        }
        coin = PlayerPrefs.GetInt("coin");
        coinText.text = coin.ToString();
        score = PlayerPrefs.GetInt("score");
        scoreText.text = score.ToString();
        moves = PlayerPrefs.GetInt("moves");
        movesText.text = moves.ToString();
        float standardScore = currentLevel * currentLevel * 1000;
        if (standardScore == 0)
        {
            standardScore = 500;
        }
        progress.fillAmount = score / standardScore;
        int lastRandom = 0;
        foreach(Transform child in transform)
        {
            var random = Random.Range(0, listValueSpawn.Count);
            while (random == lastRandom)
            {
                random = Random.Range(0, listValueSpawn.Count);
            }
            lastRandom = random;
            child.name = listValueSpawn[random].ToString();
            child.GetComponent<Ball>().OnChangeColor();
        }

        foreach (Transform child in listPre.transform)
        {
            var random = Random.Range(0, listValueSpawn.Count);
            while (random == lastRandom)
            {
                random = Random.Range(0, listValueSpawn.Count);
            }
            lastRandom = random;
            child.name = listValueSpawn[random].ToString();
            child.GetComponent<Ball>().OnChangeColor();
            child.transform.GetChild(0).tag = "Pre";
        }
        //Debug.LogError((int)(Mathf.Log(4000) / Mathf.Log(2)));
        bonus = 0;
        isControl = true;
        //GameAnalyticsManager.Instance.Log_StartLevel();
        //yield return new WaitForSeconds(1);
        //UnityAdsManager.instance.ShowRewardedVideo();
        //yield return new WaitForSeconds(3);
        //UnityAdsManager.instance.ShowRewardedVideo();
    }

    private static string FormatNumber(long num)
    {
        // Ensure number has max 3 significant digits (no rounding up can happen)
        long i = (long)Mathf.Pow(10, (int)Mathf.Max(0, Mathf.Log10(num) - 2));
        num = num / i * i;

        if (num >= 1000000000)
            return (num / 1000000000D).ToString("0.##") + "B";
        if (num >= 1000000)
            return (num / 1000000D).ToString("0.##") + "M";
        if (num >= 1000)
            return (num / 1000D).ToString("0.##") + "K";

        return num.ToString("#,0");
    }

    public static bool isSpawn = false;
    public void SpawnBall(GameObject target, int total, GameObject other)
    {
        if (!isSpawn)
        {
            isSpawn = true;
            float contactPoint = (float)(target.transform.GetSiblingIndex() + other.transform.GetSiblingIndex()) / 2;
            target.transform.DOMoveY(contactPoint, 0.3f).SetEase(Ease.Linear);
            target.transform.DORotate(new Vector3(120, 180, 0), 0.3f).OnComplete(() =>
            {
                var getCode = (int)(Mathf.Log(total/2) / Mathf.Log(2));
                var effect = Instantiate(dieEffect, target.transform.position, Quaternion.identity);
                effect.GetComponent<ParticleSystem>().startColor = listColor[getCode];
                Vibrate();
            });
            other.transform.DOMoveY(contactPoint, 0.3f).SetEase(Ease.Linear); ;
            other.transform.DORotate(new Vector3(-120, 180, 0), 0.3f);
            Destroy(other, 0.3f);
            if (lastMove == moves)
            {
                bonus++;
                Debug.Log(bonus);
            }
            lastMove = moves;
            score += total * bonus * multitask;
            long roundNum = (long)score;
            scoreText.text = FormatNumber(roundNum);
            scorePopup.transform.DOKill();
            scorePopup.gameObject.SetActive(true);
            scorePopup.transform.localScale = Vector3.one * 0.1f;
            scorePopup.transform.DOScale(Vector3.one, 0.2f).OnComplete(() =>
            {
                scorePopup.transform.DOScale(Vector3.one * 0.9f, 0.2f).OnComplete(() =>
                {
                    scorePopup.transform.DOScale(Vector3.one * 0.9f, 1f).OnComplete(() =>
                    {
                        scorePopup.gameObject.SetActive(false);
                    });
                });
            });
            scorePopup.text = "+" + total;
            if (bonus >= 2)
            {
                bonusPopup.transform.DOKill();
                bonusPopup.gameObject.SetActive(true);
                bonusPopup.transform.localScale = Vector3.one * 0.1f;
                bonusPopup.transform.DOScale(Vector3.one, 0.2f).OnComplete(() =>
                {
                    bonusPopup.transform.DOScale(Vector3.one * 0.9f, 0.2f).OnComplete(() =>
                    {
                        bonusPopup.transform.DOScale(Vector3.one * 0.9f, 1f).OnComplete(() =>
                        {
                            bonusPopup.gameObject.SetActive(false);
                        });
                    });
                });
                bonusPopup.text = "COMBO x " + bonus;
            }
            //Debug.LogError(multitask);
            if (multitask >= 2)
            {
                multitaskPopup.transform.DOKill();
                multitaskPopup.gameObject.SetActive(true);
                multitaskPopup.transform.DOScale(Vector3.one, 0.2f).OnComplete(() =>
                {
                    multitaskPopup.transform.DOScale(Vector3.one * 0.9f, 0.2f).OnComplete(() =>
                    {
                        multitaskPopup.transform.DOScale(Vector3.one * 0.9f, 1f).OnComplete(() =>
                        {
                            multitaskPopup.gameObject.SetActive(false);
                        });
                    });
                });
                multitaskPopup.text = "MULTITASK x " + multitask;
            }
            coin += total * bonus;
            coinText.text = coin.ToString();
            coinText.transform.parent.DOScale(Vector3.one * 1.2f, 0.2f).SetLoops(2, LoopType.Yoyo);
            target.name = total.ToString();
            float standardScore = currentLevel * currentLevel * 1000;
            if(standardScore == 0)
            {
                standardScore = 500;
            }
            progress.fillAmount = score / standardScore;
            if (score >= standardScore)
            {
                Win();
            }
            spawning = StartCoroutine(delaySpawn(target));
        }
    }

    int countRandom;
    public List<int> checkExist = new List<int>();
    IEnumerator delaySpawn(GameObject target)
    {
        yield return new WaitForSeconds(0.3f);
        target.GetComponent<Ball>().OnChangeColor();
        //var effect = Instantiate(dieEffect, target.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.01f);
        var numSpawn = 9 - transform.childCount;
        if (numSpawn == 0)
        {
            yield break;
        }
        listChoose.Clear();
        listNotChoose.Clear();
        foreach (Transform child in transform)
        {
            listNotChoose.Add(child.gameObject);
        }
        //for (int i = 0; i < listValueCheck.Count; i++)
        //{
        //    valueCheck temp = listValueCheck[i];
        //    temp.count = 0;
        //    listValueCheck[i] = temp;
        //}
        checkExist.Clear();
        foreach (var item in listNotChoose)
        {
            try
            {
                var num = int.Parse(item.name);
                if (!checkExist.Contains(num))
                {
                    checkExist.Add(num);
                }
                else if (checkExist.Contains(num))
                {
                    checkExist.Remove(num);
                }
            }
            catch
            {
                Destroy(item.gameObject);
                spawning = StartCoroutine(delaySpawn(transform.GetChild(0).gameObject));
                yield break;
            }
        }
        GameObject currentBiggest = null;
        foreach (var item in listNotChoose)
        {
            //for (int i = 0; i < listValueCheck.Count; i++)
            //{
            //    if (listValueCheck[i].num == int.Parse(item.name))
            //    {
            //        valueCheck temp = listValueCheck[i];
            //        temp.count++;
            //        listValueCheck[i] = temp;
            //    }
            //}
            if (int.Parse(item.name) >= biggestNum)
            {
                biggestNum = int.Parse(item.name);
                currentBiggest = item;
            }
        }
        int count = 0;

        //foreach (var item in listValueCheck)
        //{
        //    if (item.count == 1)
        //    {
        //        count++;
        //        if (count > 3)
        //        {
        //            var ballSpawn = Instantiate(ballPrefab);
        //            ballSpawn.transform.parent = listPre.transform;
        //            ballSpawn.transform.SetAsLastSibling();
        //            ballSpawn.name = listValueSpawn[item.num].ToString();
        //            ballSpawn.GetComponent<Ball>().OnChangeColor();
        //            ballSpawn.transform.DOMoveY(10, 0.1f).OnComplete(() =>
        //            {
        //                ballSpawn.transform.DOMoveX(-listPre.transform.childCount, 0.2f).SetEase(Ease.Linear);
        //            });
        //            numSpawn--;
        //            if (numSpawn <= 0)
        //            {
        //                break;
        //            }
        //        }
        //    }
        //}
        int lastRandom = 0;
        if (biggestNum >= listTasks[currentTask].num)
        {
            var tempNum = listTasks[currentTask].num;
            listNotChoose.Remove(currentBiggest);
            currentBiggest.transform.DOMoveZ(-2, 0.3f);
            currentBiggest.transform.DOShakeScale(1.5f).OnComplete(() =>
            {
                currentBiggest.transform.DOScale(Vector3.one * 0.1f, 0.2f);
                Destroy(currentBiggest, 0.2f);
                var getCode = (int)(Mathf.Log(tempNum) / Mathf.Log(2));
                var spawnEffect = Instantiate(blast, target.transform.position, Quaternion.identity);
                spawnEffect.SetActive(true);
                Destroy(spawnEffect, 5);
                //spawnEffect.GetComponent<ParticleSystem>().startColor = listColor[getCode];
                //spawnEffect.transform.localScale = Vector3.one * 2;
                Vibrate();
            });
            yield return new WaitForSeconds(1.7f);
            currentTask++;
            PlayerPrefs.SetInt("currentTask", currentTask);
            listValueSpawn = listTasks[currentTask].listValueInTask;
            countRandom = listValueSpawn.Count * 3;
            listValueCheck.Clear();
            foreach (var item in listValueSpawn)
            {
                valueCheck newItem = new valueCheck();
                newItem.num = item;
                listValueCheck.Add(newItem);
            }
            var mission = listTasks[currentTask].num;
            string missionText;
            if (mission >= 4000)
            {
                mission /= 1000;
                missionText = mission.ToString() + "K";
            }
            else
            {
                missionText = mission.ToString();
            }
            taskText.text = "REACH " + missionText;
            taskText.transform.DOScale(Vector3.one * 1.2f, 0.5f).OnComplete(() =>
            {
                taskText.transform.DOScale(Vector3.one, 0.3f);
                LoadScene();
            });
        }
        while (numSpawn > 0)
        {
            var ballSpawn = Instantiate(ballPrefab);
            ballSpawn.transform.parent = listPre.transform;
            ballSpawn.transform.SetAsLastSibling();
            countRandom--;
            var random = Random.Range(0, listValueSpawn.Count);
            if (countRandom <= 6)
            {
                var randomDirSearch = Random.Range(0, 10);
                if (randomDirSearch > 5)
                {
                    for (int i = 0; i < listValueCheck.Count; i++)
                    {
                        if (i <= 2 && listValueCheck[i].count < 2)
                        {
                            random = i;
                            valueCheck modItem = listValueCheck[random];
                            modItem.count++;
                            listValueCheck[random] = modItem;
                        }
                    }
                }
                else
                {
                    for (int i = listValueCheck.Count; i > 0; i--)
                    {
                        if (i <= 2 && listValueCheck[i].count < 2)
                        {
                            random = i;
                            valueCheck modItem = listValueCheck[random];
                            modItem.count++;
                            listValueCheck[random] = modItem;
                        }
                    }
                }
                if (countRandom == 0)
                {
                    countRandom = listValueSpawn.Count * 3;
                }
            }
            while (random == lastRandom)
            {
                random = Random.Range(0, listValueSpawn.Count + 1);
            }
            lastRandom = random;
            random--;
            if(random < 0)
            {
                random = 0;
            }
            valueCheck newItem = listValueCheck[random];
            newItem.count++;
            listValueCheck[random] = newItem;
            var num = listValueSpawn[random];
            if(checkExist.Count >= 9)
            {
                Lose();
            }
            if (checkExist.Count > 1)
            {
                foreach(var item in checkExist)
                {
                    if(listValueSpawn.Contains(item))
                    {
                        num = item;
                        break;
                    }
                }
            }
            ballSpawn.name = num.ToString();
            ballSpawn.GetComponent<Ball>().OnChangeColor();
            ballSpawn.transform.DOMoveY(10, 0.1f).OnComplete(() =>
            {
                ballSpawn.transform.DOMoveX(-listPre.transform.childCount, 0.2f).SetEase(Ease.Linear);
            });
            var ballPick = listPre.transform.GetChild(0).gameObject;
            ballPick.transform.parent = transform;
            ballPick.transform.SetAsLastSibling();
            ballPick.transform.GetChild(0).tag = "Player";
            //ballPick.transform.DOMoveY(transform.childCount, 0.6f);
            listChoose.Add(ballPick);
            numSpawn--;

            for (int i = 0; i < listChoose.Count; i++)
            {
                //listChoose[i].transform.SetSiblingIndex(i);
                listChoose[i].GetComponent<SphereCollider>().enabled = true;
                StartCoroutine(delayAnim(listChoose[i]));
                foreach (var item in listNotChoose)
                {
                    item.transform.DOMoveY(item.transform.GetSiblingIndex(), 0.6f);
                }
            }
            foreach (Transform item in listPre)
            {
                item.transform.DOMoveX(-item.transform.GetSiblingIndex(), 0.6f);
            }
        }
        checkExist.Clear();
        foreach (var item in listNotChoose)
        {
            try
            {
                var num = int.Parse(item.name);
                if (!checkExist.Contains(num))
                {
                    checkExist.Add(num);
                }
                else if (checkExist.Contains(num))
                {
                    checkExist.Remove(num);
                }
            }
            catch
            {
                Destroy(item.gameObject);
                spawning = StartCoroutine(delaySpawn(transform.GetChild(0).gameObject));
                yield break;
            }
        }
        if (checkExist.Count >= 9)
        {
            Lose();
        }
        spawning = null;
    }

    Vector3 firstP;
    public Transform target;

    private void Update()
    {
        if (isStartGame && isControl)
        {
            if (Input.GetMouseButton(0))
            {
                OnMouseDrag();
            }

            if (Input.GetMouseButtonUp(0))
            {
                OnMouseUp();
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    public GameObject firstHit;
    public RaycastHit[] hits;
    public List<Transform> listHit = new List<Transform>();
    void OnMouseDrag()
    {
        isDrag = true;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        var screenPoint = Input.mousePosition;
        screenPoint.z = 12.0f;
        hand.SetActive(true);
        hand.GetComponent<RectTransform>().position = worldToUISpace(canvas, Camera.main.ScreenToWorldPoint(screenPoint));

        if (Physics.Raycast(ray, out hit, 1000, dragMask))
        {
            if (hit.transform.CompareTag("Player"))
            {
                firstP = hit.point;
                if(firstHit == null)
                {
                    firstHit = hit.transform.gameObject;
                    hit.transform.parent.GetComponent<Ball>().isChoose = true;
                    if (currentBG == 2)
                    {
                        hit.transform.parent.GetComponent<QuickOutline>().OutlineColor = Color.white;
                    }
                    else
                    {
                        hit.transform.parent.GetComponent<QuickOutline>().OutlineColor = Color.black;
                    }
                    hit.transform.parent.GetComponent<QuickOutline>().enabled = true;
                    Vibrate();
                }
                if (hit.transform.gameObject != firstHit)
                {
                    hits = Physics.RaycastAll(firstHit.transform.position, (hit.transform.position - firstHit.transform.position), Mathf.Abs(hit.transform.position.y - firstHit.transform.position.y) - 0.5f, dragMask);
                    listHit.Clear();
                    foreach (var item in hits)
                    {
                        listHit.Add(item.transform);
                    }
                    listHit.Add(firstHit.transform);
                    foreach (Transform child in transform)
                    {
                        bool isExist = false;
                        foreach (var item in listHit)
                        {
                            if (item.transform.parent.position == child.transform.position)
                            {
                                isExist = true;
                                break;
                            }
                        }
                        if (isExist)
                        {
                            child.transform.GetComponent<Ball>().isChoose = true;
                            if (currentBG == 2)
                            {
                                hit.transform.parent.GetComponent<QuickOutline>().OutlineColor = Color.white;
                            }
                            else
                            {
                                hit.transform.parent.GetComponent<QuickOutline>().OutlineColor = Color.black;
                            }
                            child.transform.GetComponent<QuickOutline>().enabled = true;
                            Vibrate();
                        }
                        else
                        {
                            child.transform.GetComponent<Ball>().isChoose = false;
                            child.transform.GetComponent<QuickOutline>().enabled = false;
                        }
                    }
                }
                else
                {
                    foreach (Transform child in transform)
                    {
                        if (hit.transform.parent.position == child.transform.position)
                        {
                            child.transform.GetComponent<Ball>().isChoose = true;
                            if (currentBG == 2)
                            {
                                hit.transform.parent.GetComponent<QuickOutline>().OutlineColor = Color.white;
                            }
                            else
                            {
                                hit.transform.parent.GetComponent<QuickOutline>().OutlineColor = Color.black;
                            }
                            child.transform.GetComponent<QuickOutline>().enabled = true;
                        }
                        else
                        {
                            child.transform.GetComponent<Ball>().isChoose = false;
                            child.transform.GetComponent<QuickOutline>().enabled = false;
                        }
                    }
                }
            }
        }
    }

    int lastMove = -1;
    void OnMouseUp()
    {
        if (isDrag)
        {
            bonus = 1;
            firstP = Vector3.zero;
            firstHit = null;
            isDrag = false;
            hand.SetActive(false);
            multitask = listChoose.Count;
            listChoose.Clear();
            listNotChoose.Clear();
            foreach (Transform child in transform)
            {
                if(child.GetComponent<Ball>().isChoose)
                {
                    listChoose.Add(child.gameObject);
                }
                else
                {
                    listNotChoose.Add(child.gameObject);
                }
            }
            for (int i = 0; i < listChoose.Count; i++)
            {
                listChoose[i].transform.SetAsLastSibling();
                StartCoroutine(delayAnim(listChoose[i]));
                foreach(var item in listNotChoose)
                {
                    item.transform.DOMoveY(item.transform.GetSiblingIndex(), 0.6f);
                }
            }
            if(listChoose.Count >= 1)
            {
                moves++;
                movesText.text = moves.ToString();
            }
        }
    }

    IEnumerator delayAnim(GameObject target)
    {
        isControl = false;
        target.GetComponent<SphereCollider>().enabled = false;
        target.transform.DOMoveX(2, 0.2f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.25f);
        if (target != null)
        {
            target.transform.DOMoveY(target.transform.GetSiblingIndex(), 0.2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                target.transform.DOMoveX(0, 0.2f).SetEase(Ease.Linear);
                target.GetComponent<Ball>().isChoose = false;
                target.GetComponent<QuickOutline>().enabled = false;
            });
        }
        //listChoose.Clear();
        //listNotChoose.Clear();
        yield return new WaitForSeconds(0.4f);
        if(target != null)
        target.GetComponent<SphereCollider>().enabled = true;
        isSpawn = false;
        isControl = true;
    }

    public void Vibrate()
    {
        if (!UnityEngine.iOS.Device.generation.ToString().Contains("5") && !isVibrate)
        {
            isVibrate = true;
            StartCoroutine(delayVibrate());
            MMVibrationManager.Haptic(HapticTypes.LightImpact);
        }
    }

    IEnumerator delayVibrate()
    {
        yield return new WaitForSeconds(0.2f);
        isVibrate = false;
    }

    public Vector3 worldToUISpace(Canvas parentCanvas, Vector3 worldPos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        Vector2 movePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera, out movePos);
        return parentCanvas.transform.TransformPoint(movePos);
    }

    public void ButtonStartGame()
    {
        startGameMenu.SetActive(false);
        isStartGame = true;
    }

    bool isCheckShop = false;
    public void ButtonShopMenu()
    {
        shopMenu.SetActive(true);
        if (!isCheckShop)
        {

        }
    }

    public void ExitShop()
    {
        shopMenu.SetActive(false);
    }

    public void Win()
    {
        //StopAllCoroutines();
        StartCoroutine(DelayWin());
    }

    IEnumerator DelayWin()
    {
        if (isStartGame)
        {
            //isStartGame = false;
            isControl = false;
            isDrag = false;
            currentLevel++;
            PlayerPrefs.SetInt("currentLevel", currentLevel);
            yield return new WaitForSeconds(2f);
            while(spawning != null)
            {
                yield return null;
            }
            nextLevelText.text = currentLevel.ToString();
            winPanel.SetActive(true);
            blast.SetActive(true);
            //conffeti.SetActive(true);
            PlayerPrefs.SetInt("coin", coin);
            PlayerPrefs.SetInt("score", score);
            PlayerPrefs.SetInt("moves", moves);
            yield return new WaitForSeconds(2f);
            currentLevelText.text = currentLevel.ToString();
            winPanel.SetActive(false);
            //isStartGame = true;
            isControl = true;
            blast.SetActive(false);
            float standardScore = currentLevel * currentLevel * 1000;
            if (standardScore == 0)
            {
                standardScore = 500;
            }
            progress.fillAmount = score / standardScore;
            //GameAnalyticsManager.Instance.Log_EndLevel();
            UnityAdsManager.instance.ShowRewardedVideo();
            //conffeti.SetActive(false);
            //LoadScene();
        }
    }

    public void Lose()
    {
        losePanel.SetActive(true);
        UnityAdsManager.instance.ShowRewardedVideo();
        //GameAnalyticsManager.Instance.Log_EndLevel();
    }

    public void Reset()
    {
        //PlayerPrefs.SetInt("coin", coin);
        PlayerPrefs.SetInt("score", score);
        PlayerPrefs.SetInt("moves", moves);
        SceneManager.LoadScene(0);
    }

    public void LoadScene()
    {
        //StopAllCoroutines();
        StartCoroutine(delayLoadScene());
    }

    IEnumerator delayLoadScene()
    {
        //isStartGame = false;
        //isControl = false;
        //isDrag = false;
        winPanel.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        //blast.SetActive(true);
        conffeti.SetActive(true);
        PlayerPrefs.SetInt("coin", coin);
        PlayerPrefs.SetInt("score", score);
        PlayerPrefs.SetInt("moves", moves);
        //yield return new WaitForSeconds(2f);
        //Camera.main.transform.DOMoveX(100, 1);
        //yield return new WaitForSeconds(1);
        //SceneManager.LoadScene(0);
        //foreach (Transform child in transform)
        //{
        //    listNotChoose.Add(child.gameObject);
        //}
        //foreach (Transform child in listPre.transform)
        //{
        //    listNotChoose.Add(child.gameObject);
        //}
        //bool isDestroy = false;
        //foreach (var item in listNotChoose)
        //{
        //    int num = int.Parse(item.name);
        //    if(!listValueSpawn.Contains(num))
        //    {
        //        isDestroy = true;
        //        var getCode = (int)(Mathf.Log(num / 2) / Mathf.Log(2));
        //        var effect = Instantiate(dieEffect, item.transform.position, Quaternion.identity);
        //        effect.GetComponent<ParticleSystem>().startColor = listColor[getCode];
        //        Destroy(item);
        //    }
        //}
        //if(isDestroy)
        //{
        //    StartCoroutine(delaySpawn(transform.GetChild(0).gameObject));
        //}
        OnChangeBG();
    }

    public void OnChangeBG()
    {
        currentBG++;
        if (currentBG > envColor.Count - 1)
        {
            currentBG = 0;
        }
        floor.GetComponent<Renderer>().material.color = envColor[currentBG];
        Camera.main.backgroundColor = envColor[currentBG] /** 1.05f*/;
        //RenderSettings.fogColor = envColor[currentBG] /** 1.05f*/;
        //foreach (var item in envMap)
        //{
        //    item.SetActive(false);
        //}
        //envMap[currentBG].SetActive(true);
        if (currentBG == 2)
        {
            currentLevelText.color = Color.white;
            currentLevelText.transform.parent.transform.GetChild(2).GetComponent<Text>().color = Color.white;
            taskText.color = Color.white;
            scoreText.transform.parent.transform.GetChild(0).GetComponent<Text>().color = Color.white;
            movesText.transform.parent.transform.GetChild(0).GetComponent<Text>().color = Color.white;
            scorePopup.color = Color.white;
            bonusPopup.color = Color.white;
            multitaskPopup.color = Color.white;
        }
        else
        {
            currentLevelText.color = Color.black;
            currentLevelText.transform.parent.transform.GetChild(2).GetComponent<Text>().color = Color.black;
            taskText.color = Color.black;
            scoreText.transform.parent.transform.GetChild(0).GetComponent<Text>().color = Color.black;
            movesText.transform.parent.transform.GetChild(0).GetComponent<Text>().color = Color.black;
            scorePopup.color = Color.black;
            bonusPopup.color = Color.black;
            multitaskPopup.color = Color.black;
        }
    }

    public void OnChangeMap()
    {
        if (levelInput != null)
        {
            int level = int.Parse(levelInput.text.ToString());
            PlayerPrefs.SetInt("currentLevel", level);
            SceneManager.LoadScene(0);
        }
    }

    public void ButtonNextLevel()
    {
        isStartGame = true;
        currentLevel++;
        PlayerPrefs.SetInt("currentLevel", currentLevel);
        SceneManager.LoadScene(0);
    }

    public void ChoosePlayerButton()
    {
        int index = int.Parse(EventSystem.current.currentSelectedGameObject.name);
        int check = PlayerPrefs.GetInt("Player" + index.ToString());
        if(check == 0)
        {
            var objectTarget = EventSystem.current.currentSelectedGameObject.GetComponent<ItemManager>().listStatus[0];
            string priceS = objectTarget.transform.GetChild(0).GetComponent<Text>().text;
            int price = int.Parse(priceS);
            if (coin >= price)
            {
                coin -= price;
                PlayerPrefs.SetInt("Coin", coin);
                PlayerPrefs.SetInt("Player" + index.ToString(), 1);
                objectTarget.transform.SetSiblingIndex(0);
                PlayerPrefs.SetInt("CurrentPlayer", index);
            }
        }
        if(check == 1)
        {
            PlayerPrefs.SetInt("CurrentPlayer", index);
            PlayerPrefs.SetInt("Player" + index, 2);
        }
    }

    public void ChooseBallButton()
    {
        int index = int.Parse(EventSystem.current.currentSelectedGameObject.name);
        int check = PlayerPrefs.GetInt("Ball" + index.ToString());
        if (check == 0)
        {
            var objectTarget = EventSystem.current.currentSelectedGameObject.transform.GetComponent<ItemManager>().listStatus[0];
            string priceS = objectTarget.transform.GetChild(0).GetComponent<Text>().text;
            int price = int.Parse(priceS);
            if (coin >= price)
            {
                coin -= price;
                PlayerPrefs.SetInt("Coin", coin);
                PlayerPrefs.SetInt("Ball" + index.ToString(), 1);
                objectTarget.transform.SetSiblingIndex(0);
                PlayerPrefs.SetInt("CurrentBall", index);
            }
        }
        if (check == 2)
        {
            PlayerPrefs.SetInt("CurrentBall", index);
            PlayerPrefs.SetInt("Ball" + index, 1);
        }
    }
}
