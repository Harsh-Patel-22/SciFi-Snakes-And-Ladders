using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SnakesAndLadders {

    public class Player {
        private int position;
        private Dictionary<string, Ability> abilities;

        public Player(int position) {
            this.position = position;
            abilities = new Dictionary<string, Ability>();
            //abilities["blast"] = new Ability("Blast", 1, false);
            //abilities["survive"] = new Ability("Survive", 1, false);
        }

        public Ability GetAbility(string name) {
            if (abilities.ContainsKey(name)) {
                return abilities[name];
            }
            return null;
        }

        public void AddAbility(string name, int charges) {
            //abilities[name] = new Ability(name, charges, canUse); 
            abilities[name] = new Ability(name, charges); 
        }

        //public void SetCanUse(string name ,bool canUse) {
        //    abilities[name].CanUse = canUse;
        //}

        public void UseAbility(string name) {
            abilities[name].Use();
        }

        public int GetPosition() { return position; }
        public void SetPosition(int position) {  this.position = position; }
    }

    public class Ability {
        public string Name { get; private set; }
        public int Charges { get; private set; }
        //public bool CanUse { get; set; }

        //public Ability(string name, int charges, bool canUse) {
        public Ability(string name, int charges) {
            Name = name;
            Charges = charges;
            //Charges = charges;
            //CanUse = canUse;
        }

        public void Use() {
            Charges--;
        }
    }
}