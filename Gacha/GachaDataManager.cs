using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;



[System.Serializable]
public class FollowerData {
	public string name = "";
	public string id_str = "";
	public int hp = 10;
    public int is_regular = 0;
    public int auto_id = 0;
    public float speed;
	public CharacterAttribute attribute;
	public string image_url = "";
    public string[] skillNames = new string[3];
    public int attackForce = 1;
    public int defenceForce = 0;

    public FollowerData()
    {

             name = "";
  			 id_str = "";
    		hp = 10;
    		speed = 5;
    		attribute = CharacterAttribute.Normal;
    		mage_url = "";
    }


	public FollowerData(string _name, string _id_str, int _HP, CharacterAttribute _attribute,
                int _attackForce, int _defenceForce, string _imageURL) {
		name = _name;
		id_str = _id_str;
		hp = _HP;
		attribute = _attribute;
        attackForce = _attackForce;
        defenceForce = _defenceForce;
		image_url = _imageURL;
        speed = 5;
        
	}

}
public class GachaDataManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}

    string url = "http://tsuyomilog.php.xdomain.jp/gacha.php";

    public IEnumerator SaveFollower(string _ownerId, FollowerData _followerData)
    {
        WWWForm form = new WWWForm();
        form.AddField("owner_id", _ownerId);
        form.AddField("name", _followerData.name);
        form.AddField("id_str", _followerData.id_str);
        form.AddField("hp", _followerData.hp);
        form.AddField("attack_force", _followerData.attackForce);
        form.AddField("defence_force", _followerData.defenceForce);
        for(int i = 0; i < 3; i++)
        {
            if(_followerData.skillNames[i] == null)
            {
                _followerData.skillNames[i] = "";
            }

            form.AddField("skill" + i, _followerData.skillNames[i]);
        }
        form.AddField("speed", _followerData.speed.ToString());
        form.AddField("attribute",  _followerData.attribute.GetHashCode());
        form.AddField("image_url", _followerData.image_url);
        form.AddField("is_regular", 0);
        WWW www = new WWW(url, form.data);
        yield return www; //受信
        Debug.Log(www.text);
    }

    public int DecideHp(CharacterAttribute attribute)
    {
        int hp = 0;
        switch (attribute)
        {
            case CharacterAttribute.Fire:
                hp = 20;
                break;
            case CharacterAttribute.Water:
                hp = 50;
                break;
            case CharacterAttribute.Magic:
                hp = 18;
                break;
            case CharacterAttribute.Normal:
                hp = 38;
                break;
            case CharacterAttribute.Sky:
                hp = 29;
                break;
            case CharacterAttribute.Thunder:
                hp = 22;
                break;
            case CharacterAttribute.Plant:
                hp = 26;
                break;
            default:
                hp = 1;
                break;
        }

        return hp;
    }
    public int DecideAttackForce(CharacterAttribute attribute)
    {
        int attackForce = 0;
        switch (attribute)
        {
            case CharacterAttribute.Fire:
                attackForce = 12;
                break;
            case CharacterAttribute.Water:
                attackForce = 2;
                break;
            case CharacterAttribute.Magic:
                attackForce = 2;
                break;
            case CharacterAttribute.Normal:
                attackForce = 5;
                break;
            case CharacterAttribute.Sky:
                attackForce = 7;
                break;
            case CharacterAttribute.Thunder:
                attackForce = 9;
                break;
            case CharacterAttribute.Plant:
                attackForce = 4;
                break;
            default:
                attackForce = 1;
                break;
        }

        return attackForce;
    }

    public int DecideDefenceForce(CharacterAttribute attribute)
    {
        int defenceForce = 0;
        switch (attribute)
        {
            case CharacterAttribute.Fire:
                defenceForce = 3;
                break;
            case CharacterAttribute.Water:
                defenceForce = 8;
                break;
            case CharacterAttribute.Magic:
                defenceForce = 1;
                break;
            case CharacterAttribute.Normal:
                defenceForce = 3;
                break;
            case CharacterAttribute.Sky:
                defenceForce = 6;
                break;
            case CharacterAttribute.Thunder:
                defenceForce = 1;
                break;
            case CharacterAttribute.Plant:
                defenceForce = 2;
                break;
            default:
                defenceForce = 1;
                break;
        }

        return defenceForce;
    }

    public string[] DecideSkills(CharacterAttribute attribute)
    {
        string[] skills = new string[3];
        switch (attribute)
        {
            case CharacterAttribute.Fire:
                skills[0] = "FlameRadiation";
                break;
            case CharacterAttribute.Water:
                skills[0] = "SnowStorm";
                break;
            case CharacterAttribute.Magic:
                break;
            case CharacterAttribute.Normal:
                skills[0] = "BodyBlow";
                break;
            case CharacterAttribute.Sky:
                skills[0] = "BodyBlow";
                break;
            case CharacterAttribute.Thunder:
                break;
            case CharacterAttribute.Plant:
                break;
            default:
                break;
        }

        return skills;
    }
}
