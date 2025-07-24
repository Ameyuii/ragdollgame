using UnityEngine;

public interface ICharacter
{
    int GetTeamId();
    bool IsDead();
    void ResetCharacter();
}