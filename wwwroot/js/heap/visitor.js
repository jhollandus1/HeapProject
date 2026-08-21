export { NodeVisitor, SumVisitor };

class NodeVisitor {
    VisitHeapNode(heapNode) {

    }
}

class SumVisitor extends NodeVisitor {
    constructor() {
        super();
        this.sum = 0;
    }

    VisitHeapNode(heapNode) {
        this.sum += heapNode.key;
    }

    GetSum() {
        return this.sum;
    }
}
