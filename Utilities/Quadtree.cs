using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;


namespace TotallyFair.Utilities
{
    // The main quadtree class
    class Quad<T>
    {
        public bool HasChildren = false;
        // Hold details of the boundary of this node
        private Vector2 _topLeft;
        private Vector2 _botRight;

        // Contains details of node
        public Dictionary<int, T> NodeList;

        // Children of this tree
        private Quad<T> _topLeftTree;
        private Quad<T> _topRightTree;
        private Quad<T> _botLeftTree;
        private Quad<T> _botRightTree;

        public Quad()
        {
            _topLeft = new Vector2(0, 0);
            _botRight = new Vector2(0, 0);
            NodeList = new Dictionary<int, T>();
            _topLeftTree = null;
            _topRightTree = null;
            _botLeftTree = null;
            _botRightTree = null;
        }

        public Quad(Vector2 topL, Vector2 botR)
        {
            NodeList = new Dictionary<int, T>();
            _topLeftTree = null;
            _topRightTree = null;
            _botLeftTree = null;
            _botRightTree = null;
            _topLeft = topL;
            _botRight = botR;
        }

        public void Insert(int id, Vector2 pos, T obj)
        {
            /*
             * Place Node into appropriate quad
             */

            //Prevent Infinite Recursion
            if (NodeList.ContainsKey(id)) return;

            //Two or less nodes in Quad or Quad has reached size minimum
            if (NodeList.Count <= 4 && !HasChildren || Math.Abs(_topLeft.X - _botRight.X) <= 50 && Math.Abs(_topLeft.Y - _botRight.Y) <= 50 && !HasChildren)
                    NodeList.Add(id, obj);
            else
            {
                //First take nodes of existing parent quad and insert them into child quads, then delete from parent
                foreach (KeyValuePair<int, T> parentNode in NodeList) InsertIntoChild(parentNode.Key, pos, parentNode.Value);
                NodeList.Clear();
                //Insert into child Quad
                InsertIntoChild(id, pos, obj);
                HasChildren = true;
            }   
        }

        public Dictionary<int,T> Search(int id, Vector2 pos)
        {
            /*
             * Search for a desired node by position, return dictionary that contains the id in question
             */

            // Current quad cannot contain it
            if (!InBoundary(pos))
                return null;

            if ((_topLeft.X + _botRight.X) / 2 >= pos.X)
            {
                // Indicates topLeftTree
                if ((_topLeft.Y + _botRight.Y) / 2 >= pos.Y)
                {
                    if (_topLeftTree == null && NodeList.ContainsKey(id))
                        return NodeList;
                    if (_topLeftTree == null && !NodeList.ContainsKey(id))
                        return null; //ID does not exist
                    return _topLeftTree.Search(id, pos);
                }

                // Indicates botLeftTree
                else
                {
                    if (_botLeftTree == null && NodeList.ContainsKey(id))
                        return NodeList;
                    if (_botLeftTree == null && !NodeList.ContainsKey(id))
                        return null; //ID does not exist
                    return _botLeftTree.Search(id, pos);
                }
            }
            else
            {
                // Indicates topRightTree
                if ((_topLeft.Y + _botRight.Y) / 2 >= pos.Y)
                {
                    if (_topRightTree == null && NodeList.ContainsKey(id))
                        return NodeList;
                    if (_topRightTree == null && !NodeList.ContainsKey(id))
                        return null; //ID does not exist
                    return _topRightTree.Search(id, pos);
                }

                // Indicates botRightTree
                else
                {
                    if (_botRightTree == null && NodeList.ContainsKey(id))
                        return NodeList;
                    if (_botRightTree == null && !NodeList.ContainsKey(id))
                        return null; //ID does not exist
                    return _botRightTree.Search(id, pos);
                }
            }
        }

        public bool Delete(int id, Vector2 pos)
        {
            /*
             * Delete desired node from Quad Tree
             */

            // Current quad cannot contain it
            if (!InBoundary(pos))
                return false;

            if ((_topLeft.X + _botRight.X) / 2 >= pos.X)
            {
                // Indicates topLeftTree
                if ((_topLeft.Y + _botRight.Y) / 2 >= pos.Y)
                {
                    if (_topLeftTree == null && NodeList.ContainsKey(id))
                    {
                        NodeList.Remove(id);
                        return true;
                    }
                    if (_topLeftTree == null && !NodeList.ContainsKey(id))
                        return false;
                    else return _topLeftTree.Delete(id, pos); //Check if in child
                }

                // Indicates botLeftTree
                else
                {
                    if (_botLeftTree == null && NodeList.ContainsKey(id))
                    {
                        NodeList.Remove(id);
                        return true;
                    }
                    if (_botLeftTree == null && !NodeList.ContainsKey(id))
                        return false;
                    else return _botLeftTree.Delete(id, pos); //Check if in child
                }
            }
            else
            {
                // Indicates topRightTree
                if ((_topLeft.Y + _botRight.Y) / 2 >= pos.Y)
                {
                    if (_topRightTree == null && NodeList.ContainsKey(id))
                    {
                        NodeList.Remove(id);
                        return true;
                    }
                    if (_topRightTree == null && !NodeList.ContainsKey(id))
                        return false;
                    else return _topRightTree.Delete(id, pos); //Check if in child
                }

                // Indicates botRightTree
                else
                {
                    if (_botRightTree == null && NodeList.ContainsKey(id))
                    {
                        NodeList.Remove(id);
                        return true;
                    }
                    if (_botRightTree == null && !NodeList.ContainsKey(id))
                        return false;
                    else return _botRightTree.Delete(id, pos); //Check if in child
                }
            }
        }

        public void CleanUp()
        {
            /*
             * Parse Quads and set empty quads to Null
             */
            if (_topLeftTree != null)
            {
                if (_topLeftTree.NodeList.Count == 0 && !_topLeftTree.HasChildren) _topLeftTree = null;
                else _topLeftTree.CleanUp();
            }
            if (_topRightTree != null)
            {
                if (_topRightTree.NodeList.Count == 0 && !_topRightTree.HasChildren) _topRightTree = null;
                else _topRightTree.CleanUp();
            }
            if (_botLeftTree != null)
            {
                if (_botLeftTree.NodeList.Count == 0 && !_botLeftTree.HasChildren) _botLeftTree = null;
                else _botLeftTree.CleanUp();
            }
            if (_botRightTree != null)
            {
                if (_botRightTree.NodeList.Count == 0 && !_botRightTree.HasChildren) _botRightTree = null;
                else _botRightTree.CleanUp();
            }
        }

        private void InsertIntoChild(int id, Vector2 pos, T obj)
        {
            if ((_topLeft.X + _botRight.X) / 2 >= pos.X)
            {
                // Indicates topLeftTree
                if ((_topLeft.Y + _botRight.Y) / 2 >= pos.Y)
                {
                    if (_topLeftTree == null)
                        _topLeftTree = new Quad<T>(
                            new Vector2(_topLeft.X, _topLeft.Y),
                            new Vector2((_topLeft.X + _botRight.X) / 2,
                                        (_topLeft.Y + _botRight.Y) / 2));
                    _topLeftTree.Insert(id, pos, obj);
                }

                // Indicates botLeftTree
                else
                {
                    if (_botLeftTree == null)
                        _botLeftTree = new Quad<T>(
                            new Vector2(_topLeft.X,
                                        (_topLeft.Y + _botRight.Y) / 2),
                            new Vector2((_topLeft.X + _botRight.X) / 2,
                                        _botRight.Y));
                    _botLeftTree.Insert(id, pos, obj);
                }
            }
            else
            {
                // Indicates topRightTree
                if ((_topLeft.Y + _botRight.Y) / 2 >= pos.Y)
                {
                    if (_topRightTree == null)
                        _topRightTree = new Quad<T>(
                            new Vector2((_topLeft.X + _botRight.X) / 2,
                                        _topLeft.Y),
                            new Vector2(_botRight.X,
                                        (_topLeft.Y + _botRight.Y) / 2));
                    _topRightTree.Insert(id, pos, obj);
                }

                // Indicates botRightTree
                else
                {
                    if (_botRightTree == null)
                        _botRightTree = new Quad<T>(
                            new Vector2((_topLeft.X + _botRight.X) / 2,
                                        (_topLeft.Y + _botRight.Y) / 2),
                            new Vector2(_botRight.X, _botRight.Y));
                    _botRightTree.Insert(id, pos, obj);
                }
            }
        }

        // Check if current quadtree contains the point
        private bool InBoundary(Vector2 p)
        {
            return (p.X >= _topLeft.X && p.X <= _botRight.X
                    && p.Y >= _topLeft.Y && p.Y <= _botRight.Y);
        }
    }
}