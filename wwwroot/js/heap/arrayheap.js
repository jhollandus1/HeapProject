import { HeapEvent, getIterationValue, exchangeUnderlyingValues } from './viewimplementation.js';
import { Stack } from './stack.js';
import { fixParentReference } from './utility.js';
export { ArrayHeap, ArrayHeapNode, HeapItem, GetMode };

export const USER_INPUT = "USER_INPUT";
export const RESTORE_FROM_DB = "RESTORE_FROM_DB";

export const BULK_LOAD_METHOD = "BULK_LOAD_METHOD";
export const ADD_OR_REMOVE = "ADD_OR_REMOVE";

const HeapItem = Object.freeze({
    FIRST: 1,
    LAST: 2
});


function GetMode(mode, operation) {
    if (mode == USER_INPUT && operation == BULK_LOAD_METHOD) {
        return 2000;
    }
    else if (mode == USER_INPUT && operation == ADD_OR_REMOVE) {
        return 500;
    }
    else if (mode == RESTORE_FROM_DB && operation == BULK_LOAD_METHOD) {
        return 100;
    }
    else if (mode == RESTORE_FROM_DB && operation == ADD_OR_REMOVE) {
        return 0;   // this may need to change, because 500 was needed with no discernable wait, so may need 5 here (1/20)
    }   // make it fail first, then fix it (like with add_or_remove/500)
    else {
        return 0;  // should never happen
    }
}

class HeapNode {
    Accept(nodeVisitor) {
        nodeVisitor.VisitHeapNode(this);    
    }
}

class ArrayHeapNode extends HeapNode {
    constructor(key, value) {
        super();
        this.key = key;
        this.value = value;
        this.Parent = null;       
        this.cx = -1;
        this.cy = -1;
        this.iteration = -1;        // this number is associated with the text node in the circle
    }
}

class Observable {
    constructor() {
        this.listOfViews = [];
    }
    AddView(view) {
        this.listOfViews.push(view);
    }
}

class ArrayHeap extends Observable {
    constructor() {
        super();
        this.heapArray = new Array(MaxNumberViewableElements);
        this.stackOfTxtNodes = [];
        if (testFeature1b) {
            console.log(this.heapArray);
        }
        for (let i = 0; i < this.heapArray.length; i++) {
            this.heapArray[i] = null;
        }
        if (testFeature1b) {
            console.log(this.heapArray);
            console.log("new ArrayHeap");
        }

        this.count = 0;
    }

    // TODO: rename this
    GetHeapCopy() {
        let copy = new Array(MaxNumberViewableElements);
        this.DeepCopyArray(copy, this.heapArray);
        return copy;
    }


    GetShallowHeapCopy() {
        let copy = new Array(MaxNumberViewableElements);
        this.ShallowCopyArray(copy, this.heapArray);
        return copy;        
    }

    GetNodeAtIndex(index) {
        return this.heapArray[index];
    }

    UpdateModel(heapArrayCopyForView, modifiedHeapArray) {
        if (testFeature2b) {
            console.log(heapArrayCopyForView);
            console.log(modifiedHeapArray);
            console.log("heapArrayCopyForView.length: " + heapArrayCopyForView.length);
            console.log("modifiedHeapArray.length: " + modifiedHeapArray.length);
        }
        for (let i = 0; i < modifiedHeapArray.length; i++) {
            if (heapArrayCopyForView[i] != null) {
                modifiedHeapArray[i].cx = heapArrayCopyForView[i].cx;
                modifiedHeapArray[i].cy = heapArrayCopyForView[i].cy;
                //modifiedHeapArray[i].iteration = heapArrayCopyForView[i].iteration;  // this line and line in UpdateModel necessary to avoid errors
            }
        }
        if (testFeature2b) {
            console.log(modifiedHeapArray);     // infinite parent here, change from
        }
        //heapArray[index] = treeNode;
    }

    Add(key, value, mode = USER_INPUT) {
        let newItem = new ArrayHeapNode(key, value);
        let newItemCopy = new ArrayHeapNode(key, value);

        this.heapArray[this.count] = newItem;
        this.heapArray[this.count].iteration = getIterationValue();
        newItemCopy.iteration = getIterationValue();

        if (testFeature1b) {
            console.log(this.heapArray[0]);
            console.log(this.heapArray[0].key);
            if (this.heapArray[1]?.Parent != null) {
                console.log("this.heapArray[1].Parent: " + this.heapArray[1].Parent);
            }
        }
        this.count++;
        let heapArrayCopyForView = new Array(MaxNumberViewableElements);
        this.DeepCopyArray(heapArrayCopyForView, this.heapArray);
        if (testFeature2b) {
            for (let i = 0; i < heapArrayCopyForView.length; i++) {
                console.log("heapArrayCopyForView[i]:")
                console.log(heapArrayCopyForView[i]);
                console.log("this.heapArray[i]: ");
                console.log(this.heapArray[i]);
            }
        }

        let pathToFilterUp = this.FilterUp(this.count - 1);
        if (testFeature2b) {
            console.log("path is " + pathToFilterUp);
        }

        this.NotifyAllViews(new HeapEvent("add", newItemCopy, this.count - 1,
                                            pathToFilterUp, heapArrayCopyForView, this.UpdateModel,
                                            this.heapArray, this.AddNodeToTracking, null, this.stackOfTxtNodes, mode));
    }

    DeepCopyArray(to, from) {
        for (let i = 0; i < from.length; i++) {
            if (from[i] != null) {
                to[i] = new ArrayHeapNode();
                to[i].key = from[i].key;
                to[i].value = from[i].value;
                //to[i].Parent = from[i].Parent;
                to[i].cx = from[i].cx;
                to[i].cy = from[i].cy;
                to[i].iteration = from[i].iteration
            }
            else {
                to[i] = null;
            }

            if (from[i] != null && from[i].Parent != null) {
                if (to[i].Parent == null) {
                    to[i].Parent = new ArrayHeapNode();
                }
                to[i].Parent.key = from[i].Parent.key;
                to[i].Parent.value = from[i].Parent.value;
                to[i].Parent.cx = from[i].Parent.cx;
                to[i].Parent.cy = from[i].Parent.cy;
                to[i].Parent.iteration = from[i].Parent.iteration;
            }
        }
    }

    ShallowCopyArray(to, from) {
        for (let i = 0; i < from.length; i++) {
            if (from[i] != null) {
                to[i] = new ArrayHeapNode();
                to[i].key = from[i].key;
                to[i].value = from[i].value;
                //to[i].Parent = from[i].Parent;
                to[i].cx = from[i].cx;
                to[i].cy = from[i].cy;
                to[i].iteration = from[i].iteration
            }
            else {
                to[i] = null;
            }
        }
    }

    RemoveMax() {
        let mode = USER_INPUT;  // no need to have a RESTORE_FROM_DB mode for this operation, at this time
        if (this.count === 0) {
            return null;
        }
        let max = new ArrayHeapNode(this.heapArray[0].key, this.heapArray[0].value); 

        let maxCopy = new ArrayHeapNode(this.heapArray[0].key, this.heapArray[0].value);
        maxCopy.iteration = this.heapArray[0].iteration;
        maxCopy.cx = this.heapArray[0].cx;
        maxCopy.cy = this.heapArray[0].cy;  // this information (cx and cy) will be lost otherwise

        let cx = this.heapArray[0].cx;
        let cy = this.heapArray[0].cy;
        this.heapArray[0] = null;

        // deep copy with first element nulled out; view must do replace and filter down itself
        let heapArrayForViewCopy = new Array(15);
        this.DeepCopyArray(heapArrayForViewCopy, this.heapArray);

        this.heapArray[0] = new ArrayHeapNode(-1, -1);
        this.heapArray[0].key = this.heapArray[this.count - 1].key;
        this.heapArray[0].value = this.heapArray[this.count - 1].value;
        this.heapArray[0].iteration = this.heapArray[this.count - 1].iteration;
        this.heapArray[0].cx = cx;      // the node's coordinates do not change, just the internal values
        this.heapArray[0].cy = cy;

        this.heapArray[this.count - 1] = null;
        this.count--;

        if (testFeature3a) {
            console.log("this.heapArray:");
            console.log(this.heapArray); 
        }
        
        let pathToFilterDown = this.FilterDown(0);
        this.NotifyAllViews(new HeapEvent("removeMax", maxCopy, this.count + 1, pathToFilterDown,
                                            heapArrayForViewCopy, this.UpdateModel, this.heapArray,
                                            this.GetNodeFromTracking, this.RemoveNodeFromTracking, this.stackOfTxtNodes, mode));
        return max;
    }

    AddNodeToTracking(stackOfTxtNodes, item) {
        stackOfTxtNodes.push(item);
    }

    GetNodeFromTracking(stackOfTxtNodes, heapItem) {
        if (heapItem === HeapItem.FIRST) {
            return stackOfTxtNodes[0];
        }
        else if (heapItem === HeapItem.LAST) {
            return stackOfTxtNodes[stackOfTxtNodes.length - 1];
        }
        else {
            return null;
        }
    }

    RemoveNodeFromTracking(stackOfTxtNodes) {
        return stackOfTxtNodes.pop();
    }

    BuildHeap(unsortedArray) {
        if (testFeature1b) {
            console.log("heapArray length: " + this.heapArray.length);
        }
        if (unsortedArray.length > this.heapArray.length) {
            this.heapArray = new Array(unsortedArray.length);
            if (testFeature1b) {
                console.log(this.heapArray.length);
            }
        }
        else {
            // clear out existing array
            let i = 0;
            for (i = 0; i < this.heapArray.length; i++) {
                if (testFeature1b) {
                    console.log(this.heapArray[i]);
                }
                this.heapArray[i] = null;
                if (testFeature1b) {
                    console.log(this.heapArray[i]);
                }
            }

            for (i = 0; i < unsortedArray.length; i++) {
                this.heapArray[i] = unsortedArray[i];
            }
            this.count = unsortedArray.length;
            if (testFeature1b) {
                console.log(this.heapArray);
                console.log(this.count);
            }
        }

        for (let i = Math.trunc((unsortedArray.length - 1) / 2); i >= 0; i--) {
            this.FilterDown(i);
        }
        if (testFeature1b) {
            console.log(this.heapArray);
        }
    }

    NotifyAllViews(event) {
        for (let i = 0; i < this.listOfViews.length; i++) {
            this.listOfViews[i].Notify(event);
        }
    }

    FilterUp(index) {
        let path = [];
        if (testFeature1b) {
            console.log(index);
        }

        parent = Math.trunc((index - 1) / 2);
        if (parent == -0) {
            parent = 0;
        }
        if (testFeature1b) {
            console.log(parent);
        }

        if (index !== 0) {
            this.heapArray[index].Parent = this.heapArray[parent];
        }

        while (this.heapArray[index].key > this.heapArray[parent].key) {
            if (testFeature1b) {
                console.log("index is " + index + ", parent is " + parent);
            }
            if (testFeature2b) {
                console.log("this.heapArray[index].key is " + this.heapArray[index].key + " value is " + this.heapArray[index].value);
            }

            path.push(index);
            path.push(parent);

            if (testFeature2b) {
                console.log(path);
            }

            // swap index and parent
            //
            exchangeUnderlyingValues(this.heapArray[index], this.heapArray[parent]);
            // this is correct, except for iteration - we are missing iteration values being swapped, but if we adopt
            // the other implementation, appendNewNode will be using a deepCopy (or we will have to figure out where the node comes from)

            //let temp = this.heapArray[index];
            //let tempParent = this.heapArray[parent].Parent;
            //this.heapArray[index] = this.heapArray[parent];
            //this.heapArray[parent] = temp;

            //this.heapArray[index].Parent = this.heapArray[parent];
            //this.heapArray[parent].Parent = tempParent;

            //Swap(index, parent);

            index = parent;
            parent = Math.trunc((index - 1) / 2);
        }
        return path;
    }


    FilterDown(index) {
        let path = [];
        let nextIndex = 0;
        while (index < this.count) {
            if (index * 2 + 1 >= this.count) {
                break;  // leaf node, no children
            }
            else if (index * 2 + 2 >= this.count) {// one child
                nextIndex = index * 2 + 1;
                if (this.heapArray[index].key < this.heapArray[index * 2 + 1].key) {    // fixes 14-11-10 case
                    path.push(index);
                    path.push(nextIndex);

                    exchangeUnderlyingValues(this.heapArray[nextIndex], this.heapArray[index]);
                }
                else {  // works with 14-10-11 on second branch
                    fixParentReference(nextIndex + 1, this.heapArray);                    
                    break;
                }
            }
            else if (index * 2 + 1 < this.count && index * 2 + 2 < this.count) {    // two children
                nextIndex = (this.heapArray[index * 2 + 1].key > this.heapArray[index * 2 + 2].key ? index * 2 + 1 : index * 2 + 2);

                if (this.heapArray[index].key < this.heapArray[nextIndex].key) {
                    path.push(index);
                    path.push(nextIndex);

                    exchangeUnderlyingValues(this.heapArray[nextIndex], this.heapArray[index]);
                    fixParentReference(nextIndex, this.heapArray);
                }
                else {
                    break;
                }
            }
            if (testFeature1b) {
                console.log("Index is " + index + " nextIndex is: " + nextIndex);
            }
            index = nextIndex;
        }
        return path;
    }

    Size() {
        let count = 0;
        for (let i = 0; i < this.heapArray.length; i++) {
            if (this.heapArray[i] !== null) {
                count++;
            }
            else {
                break;
            }
        }
        return count;
    }
}
