using UnityEngine;

public class RestrictToType : PropertyAttribute {

    public System.Type requiredType { get; private set; }

    public RestrictToType(System.Type type) {
        this.requiredType = type;
    }
}
