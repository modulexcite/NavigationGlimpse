﻿using Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NavigationGlimpse
{
    internal class Canvas
    {
        private const int Top = 10;
        private const int Left = 10;
        private const int StateWidth = 150;
        private const int StateHeight = 50;
        private const int StateSeparation = 40;
        private const int TransitionSeparation = 20;
        private const int TransitionStepHeight = 20;
        private const int DialogSeparation = 20;

        internal static Tuple<List<StateElement>, List<TransitionElement>> Arrange(StateDisplayInfo stateDisplayInfo)
        {
            var transitionElements = new List<TransitionElement>();
            var stateElements = new List<StateElement>();
            var stateX = Left;
            var stateY = Top;
            var crumbs = StateController.Crumbs.Select((c, i) => new { Crumb = c, Index = i })
                .ToDictionary(c => c.Crumb.State, c => new { Crumb = c.Crumb, c.Index });
            foreach (Dialog dialog in StateInfoConfig.Dialogs)
            {
                stateX = Left;
                var depths = CalculateDepths(dialog, transitionElements);
                foreach (State state in dialog.States)
                {
                    var stateElement = new StateElement(state);
                    stateElements.Add(stateElement);
                    stateElement.X = stateX;
                    stateElement.Y = stateY;
                    stateElement.W = StateWidth;
                    stateElement.H = StateHeight;
                    stateElement.Page = state.Page;
                    if (state == StateContext.State)
                    {
                        stateElement.Current = state == StateContext.State;
                        stateElement.Data = StateContext.Data;
                        stateElement.Page = stateDisplayInfo.Page;
                    }
                    stateElement.Previous = state == StateContext.PreviousState;
                    stateElement.Back = 0;
                    if (crumbs.ContainsKey(state))
                    {
                        stateElement.Back = crumbs.Count - crumbs[state].Index;
                        stateElement.Data = crumbs[state].Crumb.Data;
                    }
                    if (stateElement.Data == null)
                    {
                        stateElement.Data = new NavigationData();
                        foreach (string key in stateElement.State.Defaults.Keys)
                            stateElement.Data[key] = stateElement.State.Defaults[key];
                    }
                    ProcessTransitions(stateElement, transitionElements);
                    stateX += StateWidth + StateSeparation;
                }
                stateY += Top + StateHeight + depths.Count * TransitionStepHeight + DialogSeparation;
            }
            return new Tuple<List<StateElement>,List<TransitionElement>>(stateElements, transitionElements);
        }

        private static Dictionary<int, HashSet<int>> CalculateDepths(Dialog dialog, List<TransitionElement> transEls)
        {
            var depths = new Dictionary<int, HashSet<int>>();
            var trans = from s in dialog.States
                        from t in s.Transitions
                        orderby Math.Abs(t.To.Index - t.Parent.Index)
                        select new TransitionElement(t);
            foreach (var transitionElement in trans)
            {
                transEls.Add(transitionElement);
                CalculateDepth(depths, transitionElement);
            }
            return depths;
        }

        private static void CalculateDepth(Dictionary<int, HashSet<int>> depths, TransitionElement transEl)
        {
            var depthFound = false;
            var depth = 0;
            while (!depthFound)
            {
                depthFound = !depths.ContainsKey(depth) ||
                    !depths[depth].Any(d => transEl.A <= d && d < transEl.B);
                if (!depthFound)
                    depth++;
            }
            transEl.Depth = depth;
            if (!depths.ContainsKey(transEl.Depth))
                depths[transEl.Depth] = new HashSet<int>();
            depths[transEl.Depth].UnionWith(Enumerable.Range(transEl.A, transEl.B - transEl.A));
        }

        private static void ProcessTransitions(StateElement stateElement, List<TransitionElement> transEls)
        {
            var trans = TransByState(stateElement.State, transEls);
            var transWidth = (trans.Count() - 1) * TransitionSeparation;
            var start = stateElement.X + (StateWidth - transWidth) / 2;
            foreach (var transEl in trans)
            {
                transEl.Y = stateElement.Y + StateHeight;
                transEl.H = (transEl.Depth + 1) * TransitionStepHeight;
                transEl.SetCoords(stateElement.State, start);
                start += TransitionSeparation;
            }
        }

        private static IEnumerable<TransitionElement> TransByState(State state, List<TransitionElement> transEls)
        {
            var q = from t in transEls
                    let leftA = t.To == state && t.From.Index < t.To.Index
                    let leftB = t.From == state && t.From.Index > t.To.Index
                    let left = leftA || leftB
                    let middle = t.From == state && t.To == state
                    let right = !left && !middle
                    where t.From == state || t.To == state
                    select new { trans = t, left, right, middle };
            foreach (var t in q.Where(t => t.left).OrderBy(t => t.trans.Depth))
                yield return t.trans;
            foreach (var t in q.Where(t => t.middle).OrderByDescending(t => t.trans.Depth))
                yield return t.trans;
            foreach (var t in q.Where(t => t.middle).OrderBy(t => t.trans.Depth))
                yield return t.trans;
            foreach (var t in q.Where(t => t.right).OrderByDescending(t => t.trans.Depth))
                yield return t.trans;
        }
    }
}
