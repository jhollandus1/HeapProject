export { Stack };

class Stack {
    constructor() {
        this.array = [];
    }

    Push(item) {
        this.array.push(item);
    }

    Pop() {
        if (this.array.length > 0) {
            return this.array.pop();
        }
    }

    PeekFirst() {
        if (this.array.length > 0) {
            return this.array[0];
        }
    }

    PeekLast() {
        if (this.array.length > 0) {
            return this.array[this.array.length - 1];
        }
    }

    GetElementAt(index) {
        return this.array[index];
    }
}
