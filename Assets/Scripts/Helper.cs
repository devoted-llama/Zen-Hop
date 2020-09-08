using UnityEngine;

public static class Helper
{
    public static bool CheckRigidBodyContactsGameObjectHasComponent<T>(Rigidbody2D rb, GameObject go, int colliderSize = 2) {
        Collider2D[] contacts = new Collider2D[colliderSize];
        rb.GetContacts(contacts);
        for (int i = 0; i < contacts.Length; i++) {
            if (contacts[i] != null) {
                T contact = contacts[i].GetComponent<T>();
                if (contact != null && contact.Equals(go.GetComponent<T>())) {
                    return true;
                }
            }
        }
        return false;
    }

    public static bool CheckRigidBodyContactsHasComponent<T>(Rigidbody2D rb, int colliderSize = 2) {
        Collider2D[] contacts = new Collider2D[colliderSize];
        rb.GetContacts(contacts);
        for (int i = 0; i < contacts.Length; i++) {
            if (contacts[i] != null) {
                T contact = contacts[i].GetComponent<T>();
                if (contact != null) {
                    return true;
                }
            }
        }
        return false;
    }
}
