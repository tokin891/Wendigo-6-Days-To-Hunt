using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AdultGenericBehaviour : MonoBehaviour
{
    #region Details

    #region ID
    private int Id;
    private int id
    {
        set
        {
            Id = value;
        }
        get
        {
            return Id;
        }
    }
    public ref readonly int GetID => ref Id;
    #endregion

    #region HP
    private float Health;
    private float health
    {
        get
        {
            return Health;
        }
        set
        {
            Health = value;
        }
    }
    public ref readonly float GetHealth => ref Health;
    #endregion

    #region Type
    private TypeAdults TypeAdults = new TypeAdults();
    private TypeAdults typeAdults
    {
        get
        {
            return TypeAdults;
        }
        set
        {
            TypeAdults = value;
        }
    }
    public ref readonly TypeAdults GetTypeAdults => ref TypeAdults;
    #endregion

    #region Speed
    private float Speed = 4;
    public float SetAndGetSpeed
    {
        get
        {
            return Speed;
        }
        set
        {
            if (value > 0)
            {
                Speed = value;
            }
            else
                Speed = 0;
        }
    }
    #endregion

    #endregion

    #region Virtual & Abstract Method
    public virtual void ChangeHealth(float _health)
    {
        health = _health;
    }
    public virtual void TakeHealth(float _health)
    {
        health = health - _health;
    }
    public virtual void SetAdultType(TypeAdults _type)
    {
        typeAdults = _type;
    }
    public virtual void SetID(int _id)
    {
        id = _id;

        Debug.Log(GetID + " " + typeAdults.ToString());
    }

    private void Awake()
    {
        id = GetComponent<Photon.Pun.PhotonView>().ViewID;
    }
    #endregion
}

public enum TypeAdults
{
    Deer,
    Wolf
}
