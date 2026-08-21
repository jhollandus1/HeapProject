export { HashTable, fixParentReference };

class HashTable {
    constructor() {
        this.table = new Map();
    }

    // Insert or update a key-value pair
    set(key, value) {
        this.table.set(key, value);
    }

    // Retrieve a value by key
    get(key) {
        return this.table.has(key) ? this.table.get(key) : undefined;
    }

    // Remove a key-value pair
    remove(key) {
        return this.table.delete(key);      // returns true if deleted, false if not found
    }

    has(key) {
        return this.table.has(key);
    }

    display() {
        for (let [key, value] of this.table.entries()) {
            console.log('${key} => ${value}');
        }
    }

    size() {
        return this.table.size;
    }

    clear() {
        this.table.clear();
    }
}


// is this still necessary, with the parent fix
// Next: pass in heapArrayCopyForView for the callback to call back to this function with, since this is unavailable
function fixParentReference(childIndex, heapArrayUpdate) {
    let parentIndex = Math.trunc((childIndex - 1) / 2);
    if (parentIndex * 2 + 1 === childIndex) {   // if left child was passed in
        if (heapArrayUpdate[parentIndex * 2 + 2] !== null) {   // and if a right child exists
            // shouldn't need deep copy operation if parent is correctly set
            // tested this with 14-11-10-9
            heapArrayUpdate[parentIndex * 2 + 2].Parent = heapArrayUpdate[parentIndex]; // update the right child's parent to point to the parent
        }
    }
    // tested this branch 
    else if (parentIndex * 2 + 2 === childIndex) {  // if right child was passed in
        heapArrayUpdate[parentIndex * 2 + 1].Parent = heapArrayUpdate[parentIndex];     // update the left child to point to the parent
    }
}

