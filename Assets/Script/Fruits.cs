using UnityEngine;

public class Fruits : MonoBehaviour
{
    [SerializeField] private FruitsType fruitsType = FruitsType.さくらんぼ;
    [SerializeField] private GameObject nextFruitsPrefab;
    [SerializeField] private Rigidbody2D rigidbody2D;

    
    public int mySerialNumber;
    private static int _totalSerialNumber = 0;
    private static int _totalScore = 0;
    
    private FruitsManager _fruitsManager;
    
    private float _deadTime = 0;
    
    // private const float fruitsDropMaxPositionX = 2.0f;
    // private const float fruitsDropMinPositionX = -5.6f;
    
    private bool isDrop = false;

    private void Start()
    {
        _totalSerialNumber++;
        mySerialNumber = _totalSerialNumber;
        _fruitsManager = FruitsManager.Instance;
        isDrop = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out Fruits otherFruits))
        {
            if (otherFruits.fruitsType != fruitsType)
            {
                return;
            }

            if (fruitsType == FruitsType.スイカ)
            {
                // スイカの場合は削除するだけ
                Destroy(gameObject);
                
                // ポイントを加算
                AddScore(fruitsType);
                return;
            }
            
            if (otherFruits.mySerialNumber < mySerialNumber)
            {
                // 2つのフルーツの中心点を求める
                var myTransform = transform;
                Vector3 centerPosition = (myTransform.position + other.transform.position) / 2;
                // 2つの回転の平均を取る
                Quaternion rotation = Quaternion.Euler(0, 0, (myTransform.rotation.eulerAngles.z + other.transform.rotation.eulerAngles.z) / 2);

                // 新しいフルーツを作る                
                var newFruits = CreateNewFruits(centerPosition, rotation);
                
                // 2つの速度の平均を取る
                 var velocity = (rigidbody2D.velocity + otherFruits.rigidbody2D.velocity) / 2;
                newFruits.rigidbody2D.velocity = velocity;
                
                // 2つの角速度の平均を取る
                float angularVelocity = (rigidbody2D.angularVelocity + otherFruits.rigidbody2D.angularVelocity) / 2;
                newFruits.rigidbody2D.angularVelocity = angularVelocity;
                AddScore(fruitsType);
            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // 対象がデッドラインの場合
        if (other.gameObject.name == "DeadLine")
        {
            // ゲームが終了していない場合
            if (!_fruitsManager.GetIsFinish())
            {
                _deadTime += Time.deltaTime;
                if (_deadTime > 3)
                {
                    _fruitsManager.GameOver();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 対象がデッドラインの場合
         if (other.gameObject.name == "DeadLine")
        {
            // ゲームが終了していない場合
            if (!_fruitsManager.GetIsFinish())
            {
                _deadTime = 0;
            }
        }
    }

    private Fruits CreateNewFruits(Vector3 position, Quaternion rotation)
    {
        Debug.Log("新しいフルーツを作る");
        var newFruits = Instantiate(nextFruitsPrefab, position, rotation, _fruitsManager.GetBasket().transform);
        newFruits.TryGetComponent(out Fruits fruits);
        
        // SE
        _fruitsManager.PlaySoundSe(SoundList.Create);
        
        return fruits;
    }
    
    private void AddScore(FruitsType type)
    {
        int score = 0;
        switch (type)
        {
            case FruitsType.さくらんぼ:
                score += 2;
                break;
            case FruitsType.いちご:
                score += 4;
                break;
            case FruitsType.みかん:
                score += 8;
                break;
            case FruitsType.なし:
                score += 20;
                break;
            case FruitsType.メロン:
                score += 50;
                break;
            case FruitsType.スイカ:
                score += 200;
                break;
        }
        
        _fruitsManager.AddScore(score);
    }
}
