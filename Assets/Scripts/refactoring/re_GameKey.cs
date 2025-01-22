using UnityEngine;

public interface re_GameKey
{
    bool leftKey();
    bool rightKey();
    bool rotateKey();
    bool downKey();
    bool dropKey();
    bool bombKey();
    bool changeKey();

    public class Player1Key : re_GameKey
    {
        public bool leftKey()
        {
            if (Input.GetKeyDown("a")) return true;
            else return false;
        }
        public bool rightKey()
        {
            if (Input.GetKeyDown("d")) return true;
            else return false;
        }
        public bool rotateKey()
        {
            if (Input.GetKeyDown("w")) return true;
            else return false;
        }
        public bool downKey()
        {
            if (Input.GetKeyDown("s")) return true;
            else return false;
        }
        public bool dropKey()
        {
            if (Input.GetKeyDown("space")) return true;
            else return false;
        }
        public bool bombKey()
        {
            if (Input.GetKeyDown("v")) return true;
            else return false;
        }
        public bool changeKey()
        {
            if (Input.GetKeyDown("b")) return true;
            else return false;
        }
    }
    
    public class Player2Key : re_GameKey
    {
        public bool leftKey()
        {
            if (Input.GetKeyDown("left")) return true;
            else return false;
        }
        public bool rightKey()
        {
            if (Input.GetKeyDown("right")) return true;
            else return false;
        }
        public bool rotateKey()
        {
            if (Input.GetKeyDown("up")) return true;
            else return false;
        }
        public bool downKey()
        {
            if (Input.GetKeyDown("down")) return true;
            else return false;
        }
        public bool dropKey()
        {
            if (Input.GetKeyDown("/")) return true;
            else return false;
        }
        public bool bombKey()
        {
            if (Input.GetKeyDown(";")) return true;
            else return false;
        }
        public bool changeKey()
        {
            if (Input.GetKeyDown("'")) return true;
            else return false;
        }
    }
    
    public class Player3Key : re_GameKey
    {
        public bool leftKey()
        {
            float mouseMove = Input.GetAxis("Mouse X");
            
            if (mouseMove < 0) return true;
            else return false;
        }
        public bool rightKey()
        {
            float mouseMove = Input.GetAxis("Mouse X");

            if (mouseMove > 0) return true;
            else return false;
        }
        public bool rotateKey()
        {
            if (Input.GetMouseButtonDown(0)) return true;
            else return false;
        }
        public bool downKey()
        {
            float wheelInput = Input.GetAxis("Mouse ScrollWheel");

            if (wheelInput < 0) return true;
            else return false;
        }
        public bool dropKey()
        {
            if (Input.GetMouseButtonDown(1)) return true;
            else return false;
        }
        public bool bombKey()
        {
            if (Input.GetKeyDown(KeyCode.Keypad8)) return true;
            else return false;
        }
        public bool changeKey()
        {
            if (Input.GetKeyDown(KeyCode.Keypad9)) return true;
            else return false;
        }
    }
}
