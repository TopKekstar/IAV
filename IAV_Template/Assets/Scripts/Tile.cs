using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
  
        private int _x, _y;
        private uint _tileID;
        Tile(int x, int y, uint tileID)
        {
            _x = x;
            _y = y;
            _tileID = tileID;
        }
        public Tile getTile()
        {
            return this;
        }
        public void setX(int x)
        {
            _x = x;
        }
        public void setY(int y)
        {
            _y = y;

        }

    
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
