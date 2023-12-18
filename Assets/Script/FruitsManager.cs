using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum FruitsType
{
    最小,
    さくらんぼ,
    いちご,
    みかん,
    なし,
    メロン,
    スイカ,
    Max
}

public enum SoundList
{
    Drop,
    Create,
    GameOver,
    Max
}

public class FruitsManager : MonoBehaviour
{
    [SerializeField] private GameObject[] fruitsPrefabs;
    [SerializeField] private Image nextFruitsImage;
    [SerializeField] private Image nextNextFruitsImage;
    [SerializeField] private Sprite[] fruitsImages;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private List<GameObject> fruitsList;
    
    [SerializeField] private TextMeshProUGUI resultScoreText;
    [SerializeField] private GameObject resultObject;
    [SerializeField] private GameObject basket;
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] dropSoundList;

    private int _nextFruitsIndex;
    private int _nextNextFruitsIndex;
    private int _score;
    
    private bool _isFinish;
    private bool _isDrop;
    
    private const float FruitsDropPositionY = 3.1f;
    private const float NextFruitsDropPositionY = 310f;
    
    // このクラスをシングルトンにする
    private static FruitsManager instance;
    
    public static FruitsManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<FruitsManager>();
            }

            return instance;
        }
    }
    
    private void Start()
    {
        fruitsList = new();
        scoreText.text = "0";
        _nextFruitsIndex = Random.Range((int)FruitsType.さくらんぼ, (int)FruitsType.みかん);
        _nextNextFruitsIndex = Random.Range((int)FruitsType.さくらんぼ, (int)FruitsType.みかん);
        nextFruitsImage.sprite = fruitsImages[_nextFruitsIndex];
        nextNextFruitsImage.sprite = fruitsImages[_nextNextFruitsIndex];
        _isDrop = true;
        _isFinish = false;
    }

    private void Update()
    {
         if (_isFinish)
         {
             return;
         }
        
         Vector3 mousePosition = Input.mousePosition;
         mousePosition.z = 0;
         mousePosition.y = nextFruitsImage.transform.position.y;
         // mousePosition.y = NextFruitsDropPositionY;
         nextFruitsImage.transform.position = mousePosition;
        
         // スペースキーが押されたらフルーツの種類をランダムに生成する
         if(Input.GetKeyDown(KeyCode.Space) && _isDrop)
         {
             Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
             position.z = 0;
            
             position.y = FruitsDropPositionY;
            
             // フルーツの種類に応じてフルーツを生成する
             GameObject newFruits = Instantiate(fruitsPrefabs[_nextFruitsIndex], position, Quaternion.identity, basket.transform);
             fruitsList.Add(newFruits);
            
             // SE
             audioSource.PlayOneShot(dropSoundList[(int)SoundList.Drop]);
            
             // 生成したら次のフルーツを抽選する
             _nextFruitsIndex = _nextNextFruitsIndex;
             _nextNextFruitsIndex = Random.Range((int)FruitsType.最小, (int)FruitsType.みかん);
             nextFruitsImage.sprite = fruitsImages[_nextFruitsIndex];
             nextNextFruitsImage.sprite = fruitsImages[_nextNextFruitsIndex];
            
             // 生成したフルーツを少し傾ける
             newFruits.transform.Rotate(new Vector3(0, 0, Random.Range(-180, 180)));
            
             _isDrop = false;
             StartCoroutine(Wait());
         }
    }
    
    // コルーチンで1秒まつ
    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);
        _isDrop = true;
    }
    
    /// <summary>
    /// スコアを加算する
    /// </summary>
    public void AddScore(int addScore)
    {
        _score += addScore;
        scoreText.text = _score.ToString();
    }

    /// <summary>
    /// リセットしてゲームを再開する
    /// </summary>
    public void Reset()
    {
        _score = 0;
        scoreText.text = _score.ToString();

        // フルーツを全て削除する
        foreach (Transform child in basket.transform)
        {
            Destroy(child.gameObject);
        }
        
        _nextFruitsIndex = Random.Range((int)FruitsType.最小, (int)FruitsType.みかん);
        _nextNextFruitsIndex = Random.Range((int)FruitsType.最小, (int)FruitsType.みかん);
        nextFruitsImage.sprite = fruitsImages[_nextFruitsIndex];
        nextNextFruitsImage.sprite = fruitsImages[_nextNextFruitsIndex];
        _isFinish = false;
        _isDrop = true;

        // リザルトオブジェクトを非表示にする
        resultObject.SetActive(false);
        
        _isFinish = false;
    }

    /// <summary>
    /// ゲームオーバーにする
    /// </summary>
    public void GameOver()
    {
        _isFinish = true;
        resultScoreText.text = _score.ToString();
        
        // リザルトオブジェクトを表示する
        resultObject.SetActive(true);
        
        // SE
        audioSource.PlayOneShot(dropSoundList[(int)SoundList.GameOver]);
    }
    
    /// <summary>
    /// isFinishの状況を返す
    /// </summary>
    public bool GetIsFinish()
    {
        return _isFinish;
    }
    
    /// <summary>
    /// バスケットを取得
    /// </summary>
    public GameObject GetBasket()
    {
        return basket;
    }
    
    /// <summary>
    /// サウンドを再生する
    /// </summary>
    public void PlaySoundSe(SoundList sound)
    {
        audioSource.PlayOneShot(dropSoundList[(int)sound]);
    }
}