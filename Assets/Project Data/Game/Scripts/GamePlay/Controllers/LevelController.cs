using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JellyMerge
{
    public class LevelController : MonoBehaviour
    {
        private static LevelController instance;

        [Header("Settings")]
        public int physicalCubesLimit = 100;
        public int particlesLimit = 50;

        [Header("References")]
        public MeshFilter borderMeshFilter;

        private Level level;
        private Pool cellsPool;
        private Pool cellsBackPool;
        private Pool staticCellsPool;

        private List<List<CellItem>> playgroud;
        private List<List<int>> sameColoresNeighboursMatrix;
        private List<int> colorsAmount;
        private List<CellBehaviour> lastCellsOfColor;

        private bool isTouchActive = false;
        private bool stuckResolved = true;
        private bool lastMoveHasMerge = false;

        private float particlesSpawnChance;
        private int activeCubesAmount;

        private Vector3 prevInput;
        private Vector3 startInput;

        private Swipe swipeControls;
        private Vector2Int lastMove;

        public static Vector2Int LastMove
        {
            get { return instance.lastMove; }
        }


        public static float ParticleSpawnChance
        {
            get { return instance.particlesSpawnChance; }
        }


        private void Awake()
        {
            instance = this;

            cellsBackPool = PoolManager.GetPoolByName("CellBackground");
            cellsPool = PoolManager.GetPoolByName("CellBehaviour");
            staticCellsPool = PoolManager.GetPoolByName("StaticCell");

            swipeControls = gameObject.AddComponent<Swipe>();
        }

        #region Level Loading

        public static void Load(Level levelToLoad)
        {
            instance.level = levelToLoad;
            instance.LoadLevel();

            instance.isTouchActive = true;

            instance.CheckForLevelComplete();
        }

        private void LoadLevel()
        {
            stuckResolved = false;

            playgroud = new List<List<CellItem>>();
            colorsAmount = new List<int>();
            lastCellsOfColor = new List<CellBehaviour>();
            cellsPool.ReturnToPoolEverything();
            cellsBackPool.ReturnToPoolEverything();
            staticCellsPool.ReturnToPoolEverything();

            for (int i = 0; i < System.Enum.GetNames(typeof(ColorId)).Length; i++)
            {
                colorsAmount.Add(0);
                lastCellsOfColor.Add(null);
            }

            activeCubesAmount = 0;
            int xLength = level.items.Length;
            int zLength = level.items[0].ints.Length;

            for (int i = 0; i < xLength; i++)
            {
                List<CellItem> currentLine = new List<CellItem>();
                for (int j = 0; j < zLength; j++)
                {
                    CellItem newCell = new CellItem();
                    ColorId currentColorID = (ColorId)level.items[i].ints[j];

                    if (currentColorID == ColorId.None)
                    {
                        newCell.InitEmptyItem();
                    }
                    else if (currentColorID == ColorId.Static)
                    {
                        newCell.InitStaticItem();
                        staticCellsPool.GetPooledObject(new Vector3(i, 0f, j));

                        colorsAmount[(int)ColorId.Static]++;
                    }
                    else
                    {
                        activeCubesAmount++;
                        CellBehaviour cell = cellsPool.GetPooledObject(new Vector3(i, 0f, j)).GetComponent<CellBehaviour>();
                        cell.Init(currentColorID, xLength * zLength < physicalCubesLimit ? CellBehaviour.GraphicsType.Physical : CellBehaviour.GraphicsType.Simple);

                        //counting amounts of all cells
                        colorsAmount[level.items[i].ints[j]]++;
                        newCell.InitColoredItem(cell);
                    }

                    currentLine.Add(newCell);
                    cellsBackPool.GetPooledObject(new PooledObjectSettings().SetPosition(new Vector3(i, 0.01f, j)).SetEulerRotation(new Vector3(90f, 0f)));
                }

                playgroud.Add(currentLine);
            }

            OnLevelLoaded();
        }

        private void OnLevelLoaded()
        {
            Vector3 levelCenter = new Vector3(-0.5f, 0f, -0.5f) + new Vector3(level.size.x * 0.5f, 0f, level.size.y * 0.5f);
            CameraController.Init(levelCenter, level.size);

            GenerateBorder(CameraController.FrustrumSize);
        }

        private void GenerateBorder(Vector2 environmentSize)
        {
            List<Vector3> verts = new List<Vector3>();
            Vector2[] uvs = new Vector2[verts.Count];
            List<int> tris = new List<int>();

            // top plane
            float mostLeftX = -(environmentSize.x - level.size.x) * 0.5f - 0.5f;
            float mostRightX = level.size.x - mostLeftX - 1f;
            float mostLowZ = -(environmentSize.y - level.size.y) * 0.5f;
            float mostHighZ = level.size.y - mostLowZ;

            verts.Add(new Vector3(mostLeftX, 1f, mostHighZ));
            verts.Add(new Vector3(-0.5f, 1f, mostHighZ));
            verts.Add(new Vector3(level.size.x - 0.5f, 1f, mostHighZ));
            verts.Add(new Vector3(mostRightX, 1f, mostHighZ));

            verts.Add(new Vector3(mostLeftX, 1f, level.size.y - 0.5f));
            verts.Add(new Vector3(-0.5f, 1f, level.size.y - 0.5f));
            verts.Add(new Vector3(level.size.x - 0.5f, 1f, level.size.y - 0.5f));
            verts.Add(new Vector3(mostRightX, 1f, level.size.y - 0.5f));

            tris.AddRange(new int[] { 0, 5, 4 });
            tris.AddRange(new int[] { 0, 1, 5 });
            tris.AddRange(new int[] { 1, 2, 6 });
            tris.AddRange(new int[] { 1, 6, 5 });
            tris.AddRange(new int[] { 2, 3, 7 });
            tris.AddRange(new int[] { 2, 7, 6 });


            verts.Add(new Vector3(mostLeftX, 1f, -0.5f));
            verts.Add(new Vector3(-0.5f, 1f, -0.5f));
            verts.Add(new Vector3(level.size.x - 0.5f, 1f, -0.5f));
            verts.Add(new Vector3(mostRightX, 1f, -0.5f));

            tris.AddRange(new int[] { 4, 9, 8 });
            tris.AddRange(new int[] { 4, 5, 9 });
            tris.AddRange(new int[] { 6, 7, 11 });
            tris.AddRange(new int[] { 6, 11, 10 });


            verts.Add(new Vector3(mostLeftX, 1f, mostLowZ));
            verts.Add(new Vector3(-0.5f, 1f, mostLowZ));
            verts.Add(new Vector3(level.size.x - 0.5f, 1f, mostLowZ));
            verts.Add(new Vector3(mostRightX, 1f, mostLowZ));

            tris.AddRange(new int[] { 8, 9, 13 });
            tris.AddRange(new int[] { 8, 13, 12 });
            tris.AddRange(new int[] { 9, 10, 14 });
            tris.AddRange(new int[] { 9, 14, 13 });
            tris.AddRange(new int[] { 10, 15, 14 });
            tris.AddRange(new int[] { 10, 11, 15 });


            // vertical borders

            verts.Add(new Vector3(-0.5f, 1f, level.size.y - 0.5f));                 //16
            verts.Add(new Vector3(level.size.x - 0.5f, 1f, level.size.y - 0.5f));
            verts.Add(new Vector3(-0.5f, 0f, level.size.y - 0.5f));
            verts.Add(new Vector3(level.size.x - 0.5f, 0f, level.size.y - 0.5f));
            verts.Add(new Vector3(-0.5f, 1f, -0.5f));                               //20
            verts.Add(new Vector3(level.size.x - 0.5f, 1f, -0.5f));
            verts.Add(new Vector3(-0.5f, 0f, -0.5f));
            verts.Add(new Vector3(level.size.x - 0.5f, 0f, -0.5f));                 //23

            tris.AddRange(new int[] { 20, 16, 18 });
            tris.AddRange(new int[] { 20, 18, 22 });
            tris.AddRange(new int[] { 16, 17, 19 });
            tris.AddRange(new int[] { 16, 19, 18 });
            tris.AddRange(new int[] { 17, 21, 19 });
            tris.AddRange(new int[] { 19, 21, 23 });


            Mesh mesh = new Mesh();
            mesh.vertices = verts.ToArray();
            mesh.triangles = tris.ToArray();
            mesh.RecalculateNormals();


            borderMeshFilter.mesh = mesh;
        }

        #endregion

        #region Touch and Movement

        private void Update()
        {
            if (!isTouchActive)
                return;

            if (swipeControls.SwipeRight)
            {
                OnSwipe(Vector2Int.right);
            }
            else if (swipeControls.SwipeLeft)
            {
                OnSwipe(Vector2Int.left);
            }
            else if (swipeControls.SwipeTop)
            {
                OnSwipe(Vector2Int.up);
            }
            else if (swipeControls.SwipeBottom)
            {
                OnSwipe(Vector2Int.down);
            }

#if UNITY_EDITOR

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                OnSwipe(Vector2Int.up);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                OnSwipe(Vector2Int.right);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                OnSwipe(Vector2Int.down);
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                OnSwipe(Vector2Int.left);
            }
#endif
        }

        private void ResetStartTouchPostion()
        {
            startInput = Input.mousePosition;
            prevInput = Input.mousePosition;
        }

        private void OnSwipe(Vector2Int direction)
        {
            ResetStartTouchPostion();

            // Play swipe sound
            AudioController.PlaySound(AudioController.Sounds.swipeClip);

            StartCoroutine(Move(direction));

            if (!UIController.GamePage.IsPageDisplayed)
            {
                GameController.OnTapPerformed();
            }
        }

        private IEnumerator Move(Vector2Int direction)
        {
            lastMove = direction;
            isTouchActive = false;
            lastMoveHasMerge = false;

            int linesAmount = GetLinesAmount(direction);
            int itemsInLine = GetItemsInLineAmount(direction);

            bool squishSoundHasPlayed = false;

            particlesSpawnChance = Mathf.Clamp01((float)instance.particlesLimit / instance.activeCubesAmount);

            for (int line = 0; line < linesAmount; line++)
            {
                bool hitAnimationInited = false;
                int pathLength = 0;
                ColorId prevColor = ColorId.None;

                for (int itemOnLine = 0; itemOnLine < itemsInLine; itemOnLine++)
                {
                    CellItem currentItem = GetCell(direction, line, itemOnLine);

                    if (currentItem.ColorID == ColorId.Static)
                    {
                        pathLength = 0;
                    }

                    // if item will move
                    if (pathLength > 0 && currentItem.ColorID != ColorId.None)
                    {
                        if (prevColor != ColorId.None && prevColor != currentItem.ColorID)
                        {
                            pathLength--;
                            hitAnimationInited = false;
                        }

                        if (currentItem.ColorID != ColorId.None)
                        {
                            prevColor = currentItem.ColorID;
                        }

                        bool disableAfterMove;
                        CellItem finalItem = GetFinalCellOnPath(direction, line, itemOnLine, pathLength);

                        // if final item on path is EMPTY
                        if (finalItem.ColorID == ColorId.None || pathLength == 0)
                        {
                            finalItem.InitColoredItem(currentItem.Cell);
                            disableAfterMove = false;
                        }
                        // if final item on path is Cell of the same color
                        else
                        {
                            disableAfterMove = true;
                            lastMoveHasMerge = true;
                            colorsAmount[(int)currentItem.ColorID]--;

                            // initing animation on final cell of path (if not done before)
                            if (!hitAnimationInited)
                            {
                                finalItem.Cell.PlayHitAnimation(direction);
                                hitAnimationInited = true;

                                lastCellsOfColor[(int)finalItem.ColorID] = finalItem.Cell;
                            }

                            if (!squishSoundHasPlayed)
                            {
                                // Play squish sound
                                AudioController.PlaySound(AudioController.Sounds.squishClip);

                                squishSoundHasPlayed = true;
                            }
                        }

                        // determining whether the trail will be shown 
                        CellItem nextItem = GetCell(direction, line, Mathf.Clamp(itemOnLine + 1, 0, itemsInLine - 1));
                        bool showTrail = nextItem.ColorID == ColorId.None || nextItem.ColorID == ColorId.Static;

                        // moving current cell physically
                        MoveCell(currentItem.Cell.IndexPosition, direction * pathLength, disableAfterMove);

                        if (pathLength != 0)
                        {
                            currentItem.InitEmptyItem();
                        }
                    }
                    else
                    {
                        if (currentItem.ColorID != ColorId.None)
                        {
                            prevColor = currentItem.ColorID;
                        }
                    }

                    pathLength++;
                }
            }

            yield return new WaitForSeconds(CellBehaviour.AnimationTime);

            isTouchActive = true;

            CheckForLevelComplete();

            Vibration.Vibrate(AudioController.Vibrations.shortVibration);
        }

        // returns vertical lines amount on UP or DOWN swipe and oposite
        private int GetLinesAmount(Vector2Int direction)
        {
            if (direction == Vector2Int.up || direction == Vector2Int.down)
            {
                return level.size.x;
            }
            else
            {
                return level.size.y;
            }
        }

        // returns vertical lines items amount on UP or DOWN swipe and oposite
        private int GetItemsInLineAmount(Vector2Int direction)
        {
            if (direction == Vector2Int.up || direction == Vector2Int.down)
            {
                return level.size.y;
            }
            else
            {
                return level.size.x;
            }
        }

        private CellItem GetCell(Vector2Int direction, int line, int item)
        {
            if (direction == Vector2Int.up || direction == Vector2Int.down)
            {
                return playgroud[line][direction.y == -1 ? item : level.size.y - 1 - item];
            }
            else
            {
                return playgroud[direction.x == -1 ? item : level.size.x - 1 - item][line];
            }
        }

        private CellItem GetFinalCellOnPath(Vector2Int direction, int line, int item, int pathLength)
        {
            if (direction == Vector2Int.up || direction == Vector2Int.down)
            {
                return playgroud[line][direction.y == -1 ? item - pathLength : level.size.y - 1 - item + pathLength];
            }
            else
            {
                return playgroud[direction.x == -1 ? item - pathLength : level.size.x - 1 - item + pathLength][line];
            }
        }


        private void MoveCell(Vector2Int cellIndex, Vector2Int vector, bool disableAfterMove)
        {
            playgroud[cellIndex.x][cellIndex.y].Cell.Move(vector, disableAfterMove);
        }

        public static void OnCellDeactivated()
        {
            if (instance.activeCubesAmount > 0)
            {
                instance.activeCubesAmount--;
            }
        }

        #endregion

        #region Level Logic

        private void CheckForLevelComplete()
        {
            if (IsLevelComplete())
            {
                LevelComplete();
            }
            else if (!lastMoveHasMerge && IsLevelStucked())
            {
                ResolveStuckSituation();
            }
        }

        private bool IsLevelComplete()
        {
            bool isLevelComplete = true;

            for (int i = 2; i < colorsAmount.Count; i++)
            {
                if (colorsAmount[i] > 1 && i != (int)ColorId.None && i != (int)ColorId.Static)
                {
                    isLevelComplete = false;
                }
            }

            return isLevelComplete;
        }

        private bool IsLevelStucked()
        {
            if (stuckResolved || colorsAmount[(int)ColorId.Static] > 0)
            {
                return false;
            }

            int[] xFilledCells = new int[level.size.x];
            int[] zFilledCells = new int[level.size.y];

            int maxNumberX = 0;
            int maxNumberZ = 0;

            int notZeroItemsAmountX = 0;
            int notZeroItemsAmountZ = 0;

            for (int i = 0; i < level.size.x; i++)
            {
                for (int j = 0; j < level.size.y; j++)
                {
                    if (playgroud[i][j].ColorID != ColorId.None && playgroud[i][j].ColorID != ColorId.Static)
                    {
                        xFilledCells[i]++;
                        zFilledCells[j]++;

                        if (xFilledCells[i] > maxNumberX)
                        {
                            maxNumberX = xFilledCells[i];
                        }

                        if (zFilledCells[j] > maxNumberZ)
                        {
                            maxNumberZ = zFilledCells[j];
                        }

                        if (xFilledCells[i] == 1)
                        {
                            notZeroItemsAmountX++;
                        }

                        if (zFilledCells[j] == 1)
                        {
                            notZeroItemsAmountZ++;
                        }
                    }
                    else if (playgroud[i][j].ColorID == ColorId.Static)
                    {
                        return false;
                    }
                }
            }

            // first check for rectangle or corner figure
            if (maxNumberX != notZeroItemsAmountZ || maxNumberZ != notZeroItemsAmountX)
            {
                return false;
            }

            // trying to find rectangle
            if (FindRectangle(xFilledCells, zFilledCells))
            {
                Debug.Log("Level stucked: Rectangle");
                return true;
            }

            // corner should have not paired sides lenght
            if (maxNumberX % 2 == 0 || maxNumberZ % 2 == 0)
            {
                return false;
            }

            // trying to find corner
            if (FindCorner(xFilledCells, zFilledCells))
            {
                Debug.Log("Level stucked: Corner");
                return true;
            }

            return false;
        }

        private bool FindCorner(int[] xFilledCells, int[] zFilledCells)
        {
            int lastNumber = xFilledCells[0];
            bool increasing = xFilledCells[0] <= 1;

            for (int i = 1; i < xFilledCells.Length; i++)
            {
                if (increasing)
                {
                    if (xFilledCells[i] != lastNumber)
                    {
                        if (lastNumber == 0)
                        {
                            if (xFilledCells[i] == 1)
                            {
                                lastNumber = 1;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if (lastNumber == 1)
                        {
                            if (xFilledCells[i] <= 1 || i != xFilledCells.Length - 1)
                            {
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    if (xFilledCells[i] != lastNumber)
                    {
                        if (lastNumber > 1)
                        {
                            if (xFilledCells[i] == 1)
                            {
                                lastNumber = 1;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if (lastNumber == 1)
                        {
                            if (xFilledCells[i] == 0)
                            {
                                lastNumber = 0;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (xFilledCells[i] > 1)
                    {
                        return false;
                    }
                }
            }

            lastNumber = zFilledCells[0];
            increasing = zFilledCells[0] <= 1;

            for (int i = 1; i < zFilledCells.Length; i++)
            {
                if (increasing)
                {
                    if (zFilledCells[i] != lastNumber)
                    {
                        if (lastNumber == 0)
                        {
                            if (zFilledCells[i] == 1)
                            {
                                lastNumber = 1;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if (lastNumber == 1)
                        {
                            if (zFilledCells[i] <= 1 || i != zFilledCells.Length - 1)
                            {
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    if (zFilledCells[i] != lastNumber)
                    {
                        if (lastNumber > 1)
                        {
                            if (zFilledCells[i] == 1)
                            {
                                lastNumber = 1;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if (lastNumber == 1)
                        {
                            if (zFilledCells[i] == 0)
                            {
                                lastNumber = 0;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (zFilledCells[i] > 1)
                    {
                        return false;
                    }
                }
            }

            return CheckForChessLayout();
        }

        private bool FindRectangle(int[] xFilledCells, int[] zFilledCells)
        {

            int expectedNumber = xFilledCells[0];
            bool foundFigure = xFilledCells[0] != 0;

            for (int i = 1; i < xFilledCells.Length; i++)
            {
                if (xFilledCells[i] != expectedNumber)
                {
                    if (foundFigure && xFilledCells[i] == 0)
                    {
                        expectedNumber = 0;
                    }
                    else if (!foundFigure && expectedNumber == 0)
                    {
                        expectedNumber = xFilledCells[i];
                    }
                    else
                    {
                        return false;
                    }
                }
            }


            expectedNumber = zFilledCells[0];
            foundFigure = zFilledCells[0] != 0;

            for (int i = 1; i < zFilledCells.Length; i++)
            {
                if (zFilledCells[i] != expectedNumber)
                {
                    if (foundFigure && zFilledCells[i] == 0)
                    {
                        expectedNumber = 0;
                    }
                    else if (!foundFigure && expectedNumber == 0)
                    {
                        expectedNumber = zFilledCells[i];
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return CheckForChessLayout();
        }

        private bool CheckForChessLayout()
        {
            bool isStuked = true;

            CellItem firstItem = null;

            for (int i = 0; i < level.size.x; i++)
            {
                for (int j = 0; j < level.size.y; j++)
                {
                    if (firstItem == null && playgroud[i][j].ColorID != ColorId.None && playgroud[i][j].ColorID != ColorId.Static)
                    {
                        firstItem = playgroud[i][j];
                    }
                }
            }

            if (firstItem == null)
            {
                LevelComplete();
                return false;
            }

            sameColoresNeighboursMatrix = new List<List<int>>();

            for (int i = 0; i < level.size.x; i++)
            {
                List<int> currentLine = new List<int>();

                for (int j = 0; j < level.size.y; j++)
                {
                    currentLine.Add(-1);
                }

                sameColoresNeighboursMatrix.Add(currentLine);
            }

            isStuked = !FoundSameColoredNeighbour(firstItem.Cell);

            return isStuked;
        }

        private bool FoundSameColoredNeighbour(CellBehaviour currentCell)
        {
            sameColoresNeighboursMatrix[currentCell.IndexPosition.x][currentCell.IndexPosition.y] = 0;

            bool result = CheckNeightbourOnDirection(currentCell, Vector2Int.right);
            if (result)
                return true;

            result = CheckNeightbourOnDirection(currentCell, Vector2Int.up);
            if (result)
                return true;

            result = CheckNeightbourOnDirection(currentCell, Vector2Int.left);
            if (result)
                return true;

            result = CheckNeightbourOnDirection(currentCell, Vector2Int.down);
            if (result)
                return true;

            return false;
        }

        private bool CheckNeightbourOnDirection(CellBehaviour currentCell, Vector2Int direction)
        {
            Vector2Int neighbourIndex = currentCell.IndexPosition + direction;

            if (neighbourIndex.x >= 0 && neighbourIndex.x < level.size.x && neighbourIndex.y >= 0 && neighbourIndex.y < level.size.y)
            {
                CellItem neighbour = playgroud[neighbourIndex.x][neighbourIndex.y];

                // if neighbour has same color
                if (neighbour.ColorID == currentCell.ColorID)
                {
                    return true;
                }
                // if neighbour is of other color and it's not checked already
                else if (neighbour.ColorID != ColorId.None && neighbour.ColorID != ColorId.Static && sameColoresNeighboursMatrix[neighbourIndex.x][neighbourIndex.y] == -1)
                {
                    bool neighbourFoundSameColor = FoundSameColoredNeighbour(neighbour.Cell);

                    if (neighbourFoundSameColor)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void ResolveStuckSituation()
        {
            if (playgroud[0][0].ColorID == ColorId.None)
            {
                stuckResolved = true;

                SpawnStaticCell(new Vector2Int(0, 0));
                return;
            }

            if (playgroud[0][level.size.y - 1].ColorID == ColorId.None)
            {
                stuckResolved = true;

                SpawnStaticCell(new Vector2Int(0, level.size.y - 1));
                return;
            }

            if (playgroud[level.size.x - 1][0].ColorID == ColorId.None)
            {
                stuckResolved = true;

                SpawnStaticCell(new Vector2Int(level.size.x - 1, 0));
                return;
            }

            if (playgroud[level.size.x - 1][level.size.y - 1].ColorID == ColorId.None)
            {
                stuckResolved = true;

                SpawnStaticCell(new Vector2Int(level.size.x - 1, level.size.y - 1));
                return;
            }
        }

        private void SpawnStaticCell(Vector2Int indexPosition)
        {
            staticCellsPool.GetPooledObject(new Vector3(indexPosition.x, 0f, indexPosition.y));
            playgroud[indexPosition.x][indexPosition.y].InitStaticItem();
        }

        #endregion

        #region Final Animations
        public static void SkipLevel()
        {
            instance.StartCoroutine(instance.SkipLevelCoroutine());
        }

        private void LevelComplete()
        {
            isTouchActive = false;

            StartCoroutine(LevelCompleteAnimation());
        }

        private IEnumerator SkipLevelCoroutine()
        {
            yield return ClearCoroutine();

            GameController.OnLevelComplete();
        }

        private IEnumerator LevelCompleteAnimation()
        {
            for (int i = 2; i < colorsAmount.Count; i++)
            {
                if (lastCellsOfColor[i] != null)
                {
                    Vector2Int cellIndex = lastCellsOfColor[i].IndexPosition;
                    playgroud[cellIndex.x][cellIndex.y].InitEmptyItem();

                    lastCellsOfColor[i].Hide();

                    lastCellsOfColor[i] = null;
                }
            }

            yield return new WaitForSeconds(1f);

            Vibration.Vibrate(AudioController.Vibrations.longVibration);

            GameController.OnLevelComplete();
        }

        private Vector3 GetFireworkOffset()
        {
            return new Vector3(Random.Range(-level.size.x * 0.7f, level.size.x * 0.7f), 0f, Random.Range(-level.size.y * 0.25f, level.size.y * 0.25f));
        }

        public static void Restart()
        {
            if (instance.isTouchActive)
                instance.StartCoroutine(instance.RestartCoroutine());
        }

        private IEnumerator RestartCoroutine()
        {
            yield return ClearCoroutine();

            Load(level);
        }

        public static void ClearLevel()
        {
            instance.StartCoroutine(instance.ClearCoroutine());
        }

        private IEnumerator ClearCoroutine()
        {
            for (int i = 0; i < playgroud.Count; i++)
            {
                for (int j = 0; j < playgroud[i].Count; j++)
                {
                    if (playgroud[i][j].ColorID != ColorId.None && playgroud[i][j].ColorID != ColorId.Static)
                    {
                        playgroud[i][j].Cell.Hide();
                    }
                }
            }

            yield return new WaitForSeconds(0.8f);

            cellsPool.ReturnToPoolEverything();
            staticCellsPool.ReturnToPoolEverything();
        }

        #endregion
    }
}