//-------------------------------------------------------------------------
// <copyright file="Recipe.cs" company="Universidad Católica del Uruguay">
// Copyright (c) Programación II. Derechos reservados.
// </copyright>
//-------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;

namespace Full_GRASP_And_SOLID;

    public class Recipe : IRecipeContent // Modificado por DIP
    {
        // Cambiado por OCP
        private IList<BaseStep> steps = new List<BaseStep>();
        private bool cooked = false;
        public bool Cooked
        {
            get
            {
                return this.cooked;
            }
        }

        public Product FinalProduct { get; set; }

        // Agregado por Creator
        public void AddStep(Product input, double quantity, Equipment equipment, int time)
        {
            Step step = new Step(input, quantity, equipment, time);
            this.steps.Add(step);
        }

        // Agregado por OCP y Creator
        public void AddStep(string description, int time)
        {
            WaitStep step = new WaitStep(description, time);
            this.steps.Add(step);
        }

        public void RemoveStep(BaseStep step)
        {
            this.steps.Remove(step);
        }

        // Agregado por SRP
        public string GetTextToPrint()
        {
            string result = $"Receta de {this.FinalProduct.Description}:\n";
            foreach (BaseStep step in this.steps)
            {
                result = result + step.GetTextToPrint() + "\n";
            }

            // Agregado por Expert
            result = result + $"Costo de producción: {this.GetProductionCost()}";

            return result;
        }

        // Agregado por Expert
        public double GetProductionCost()
        {
            double result = 0;

            foreach (BaseStep step in this.steps)
            {
                result = result + step.GetStepCost();
            }

            return result;
        }
        public int GetCookTime()
        {
            int result = 0;
            foreach(BaseStep step in this.steps)
            {
                result = result + step.Time;
            }
            return result;
        }
        private TimerAdapter timerClient;
        private CountdownTimer timer;
        
        // Para lograr el ejercicio sin modificar el tipo de Recipe y las clases en CountownTimer, usé ISP.
        // Consitió en crear una clase anidada, llamada "TimerAdapter", como en el ejemplo de la lectura de ISP.
        // Esta clase anidada hace que la clase Recipe se pueda comunicar con CountdownTimer, reduciendo la posibilidad de romper el código.
        public void Cook()
        {
            this.timerClient = new TimerAdapter(this);
            this.timer = new CountdownTimer();
            this.timer.Register(this.GetCookTime(), this.timerClient);
        } 
        private class TimerAdapter: TimerClient
        {
            private Recipe recipe;
            public TimerAdapter(Recipe recipe)
            {
                this.recipe = recipe;
            }
            public void TimeOut()
            {
                this.recipe.cooked = true;
            }
        }
    }