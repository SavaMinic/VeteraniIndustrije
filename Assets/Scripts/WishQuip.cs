using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Prokuvo/WishQuip")]
public class WishQuip : ScriptableObject
{
    public GuestWish.GuestWishType wishType;
    public string[] request;
    public string[] success;
    public string[] failure;
    public string[] tooLate;

    public string GetRequest() { return request.Length == 0 ? "" : request[Random.Range(0, request.Length)]; }
    public string GetSuccess() { return success.Length == 0 ? "" : success[Random.Range(0, success.Length)]; }
    public string GetFailure() { return failure.Length == 0 ? "" : failure[Random.Range(0, failure.Length)]; }
    public string GetToolate() { return tooLate.Length == 0 ? "" : tooLate[Random.Range(0, tooLate.Length)]; }
}
