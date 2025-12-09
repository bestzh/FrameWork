using UnityEngine;
using System;

/// <summary>
/// 角色数据 - 存储角色的所有数据
/// </summary>
[Serializable]
public class CharacterData
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public int CharacterId;
    
    /// <summary>
    /// 角色名称
    /// </summary>
    public string Name;
    
    /// <summary>
    /// 等级
    /// </summary>
    public int Level = 1;
    
    /// <summary>
    /// 经验值
    /// </summary>
    public int Exp = 0;
    
    /// <summary>
    /// 生命值
    /// </summary>
    public int HP = 100;
    
    /// <summary>
    /// 最大生命值
    /// </summary>
    public int MaxHP = 100;
    
    /// <summary>
    /// 魔法值
    /// </summary>
    public int MP = 50;
    
    /// <summary>
    /// 最大魔法值
    /// </summary>
    public int MaxMP = 50;
    
    /// <summary>
    /// 力量
    /// </summary>
    public int Strength = 10;
    
    /// <summary>
    /// 敏捷
    /// </summary>
    public int Agility = 10;
    
    /// <summary>
    /// 智力
    /// </summary>
    public int Intelligence = 10;
    
    /// <summary>
    /// 装备的武器ID
    /// </summary>
    public int WeaponId = -1;
    
    /// <summary>
    /// 装备的护甲ID
    /// </summary>
    public int ArmorId = -1;
    
    /// <summary>
    /// 金币
    /// </summary>
    public int Gold = 0;
}

